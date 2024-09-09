using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Exceptions;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Results;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Poc.Kafka.PubSub;

internal sealed partial class PocKafkaPub<TKey, TValue>
{
    public async Task<BatchSendResult<TKey, TValue>> SendBatchAsync(
        IEnumerable<Message<TKey, TValue>> messages,
        string? topic = null,
        Guid? batchId = null,
        CancellationToken cancellationToken = default)
    {
        batchId ??= Guid.NewGuid();
        _logger.LogInformation("Starting to send batch {BatchId} with {MessageCount} messages.", batchId, messages.Count());

        var result = new BatchSendResult<TKey, TValue>();

        foreach (var message in messages)
        {
            await SendBatchMessageAsync(topic!, batchId.Value, message, result, cancellationToken);
        }

        _logger.LogInformation(
          "Completed sending batch {BatchId} with {MessageCount} messages. Successes: {SuccessesCount}, Failures: {FailuresCount}.",
          batchId,
          messages.Count(),
          result.Successes.Count,
          result.Failures.Count);

        // Since we are producing synchronously, at this point there will be no messages
        // in-flight and no delivery reports waiting to be acknowledged, so there is no
        // need to call producer. Flush before disposing the producer.

        return result;
    }

    private async Task SendBatchMessageAsync(
        string topic,
        Guid batchId,
        Message<TKey, TValue> messages,
        BatchSendResult<TKey, TValue> result,
        CancellationToken cancellationToken)
    {
        try
        {
            AddHeaderBatchId(batchId, messages);

            var deliveryResult = await _producerManager.SendMessageAsync(
                messages,
                topic,
                cancellationToken);

            result.Successes.Add(deliveryResult);
        }
        catch
        {
            result.Failures.Add((messages, $"An error occurred during message delivery for batch {batchId}."));
        }
    }

    public BatchSendResult<TKey, TValue> SendBatch(
        IEnumerable<Message<TKey, TValue>> messages,
        string? topic = null,
        TimeSpan? timeout = null,
        Guid? batchId = null)
    {
        batchId ??= Guid.NewGuid();
        timeout ??= TimeSpan.FromSeconds(10);

        _logger.LogInformation(
            "Starting to send batch atomic with {MessageCount} messages and timeout {Timeout}.",
            messages.Count(),
            timeout);

        var results = new ConcurrentDictionary<string, MessageResult<TKey, TValue>>();

        foreach (var message in messages)
        {
            var messageId = Guid.NewGuid().ToString();
            var result = new MessageResult<TKey, TValue>(message);
            results[messageId] = result;

            SendBatchMessage(topic!, batchId.Value, message, result);
        }

        _logger.LogInformation("Flushing producer with timeout {FlushTimeout}.", timeout.Value);

        _producerManager.Flush(timeout: timeout.Value);

        MarkUndeliveredMessagesAsFailed(results);

        var batchResult = CompileBatchResult(results);

        _logger.LogInformation(
            "Completed sending batch {BatchId} with {MessageCount} messages. Successes: {SuccessesCount}, Failures: {FailuresCount}.",
            batchId,
            messages.Count(),
            batchResult.Successes.Count,
            batchResult.Failures.Count);

        return batchResult;
    }

    private static void MarkUndeliveredMessagesAsFailed(ConcurrentDictionary<string, MessageResult<TKey, TValue>> results)
    {
        foreach (var pendingResult in results.Where(r => !r.Value.IsDelivered && !r.Value.IsError))
            pendingResult.Value.SetErrorMessage("Delivery timed out or status uncertain.");
    }

    private static BatchSendResult<TKey, TValue> CompileBatchResult(ConcurrentDictionary<string, MessageResult<TKey, TValue>> results)
    {
        var successes = results.Values.Where(r => r.IsDelivered && !r.IsError).Select(r => r.DeliveryResult).ToList();
        var failures = results.Values.Where(r => r.IsError).Select(r => (r.Message, r.ErrorMessage)).ToList();

        return new BatchSendResult<TKey, TValue>
        {
            Successes = successes!,
            Failures = failures!
        };
    }

    private void SendBatchMessage(
       string topic,
       Guid batchId,
       Message<TKey, TValue> message,
       MessageResult<TKey, TValue> messageResult)
    {
        try
        {
            AddHeaderBatchId(batchId, message);

            _producerManager.SendMessage(
                message,
                deliveryHandler: (message, deliveryReport) =>
                    BatchDeliveryHandlerDelegate(deliveryReport, messageResult),
                topic);
        }
        catch
        {
            messageResult.SetErrorMessage($"An error occurred during message delivery for batch {batchId}.");
        }
    }

    private void BatchDeliveryHandlerDelegate(
        DeliveryReport<TKey, TValue> deliveryReport,
        MessageResult<TKey, TValue> messageResult)
    {
        if (deliveryReport.Error.IsError)
        {
            LogDeliveryHandlerError(deliveryReport);
            messageResult.SetErrorMessage(deliveryReport.Error.GetErrorFormatted());
        }
        else
        {
            LogDeliveryHandlerSuccess(deliveryReport);
            messageResult.SetDeliveryResult(deliveryReport);
        }
    }

    public bool SendBatchAtomic(
        IEnumerable<Message<TKey, TValue>> messages,
        string? topic = null,
        TimeSpan? timeout = null,
        Guid? batchId = null)
    {
        try
        {
            batchId ??= Guid.NewGuid();
            timeout ??= TimeSpan.FromSeconds(30);

            _logger.LogInformation(
                 "Starting to send batch atomic with {MessageCount} messages and timeout {Timeout}, batch ID: {BatchId}.",
                 messages.Count(),
                 timeout,
                 batchId);

            _producerManager.InitTransactions(timeout: timeout.Value);
            _producerManager.BeginTransaction();

            foreach (var message in messages)
            {
                AddHeaderBatchId(batchId.Value, message);

                _producerManager.SendMessage(
                       message,
                       topic,
                       deliveryHandler: BatchAtomicDeliveryHandlerDelegate);
            }

            _logger.LogInformation(
                "Attempting to commit transaction for batch with {MessageCount} messages, batch ID: {BatchId}.",
                messages.Count(),
                batchId);

            _producerManager.CommitTransaction();
            _logger.LogInformation("Transaction committed successfully for batch {BatchId}.", batchId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during the atomic batch sending for batch {BatchId}. Transaction aborted.", batchId);

            _producerManager.AbortTransaction();

            return false;
        }
    }

    private void BatchAtomicDeliveryHandlerDelegate(DeliveryReport<TKey, TValue> deliveryReport)
    {
        if (deliveryReport.Error.IsError)
        {
            LogDeliveryHandlerError(deliveryReport);
            throw new KafkaTransactionAbortedException("One message sending failure transaction aborted.");
        }
        else
        {
            LogDeliveryHandlerSuccess(deliveryReport);
        }
    }

    private static void AddHeaderBatchId(Guid batchId, Message<TKey, TValue> message)
    {
        message.Headers ??= [];
        message.Headers.Add(ProducerConstant.HEADER_NAME_BATCH_ID, batchId.ToByteArray());
    }
}
