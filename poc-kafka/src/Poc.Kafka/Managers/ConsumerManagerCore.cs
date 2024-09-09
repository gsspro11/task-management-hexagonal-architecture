using Poc.Kafka.Common;
using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Common.Settings;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Providers;
using Poc.Kafka.Results;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;


namespace Poc.Kafka.Managers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]
internal sealed class ConsumerManagerCore<TKey, TValue> : IConsumerManagerCore<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly ITimeProvider _timeProvider;
    private readonly IDelayService _delayService;
    private readonly IConsumerConfiguration<TKey, TValue> _consumerConfiguration;
    private readonly Lazy<IConsumer<TKey, TValue>> _lazyConsumer;
    private readonly Lazy<IProducer<TKey, TValue>> _lazyProducer;

    private readonly ConcurrentDictionary<string, Task> _partitionTasks = new();

    internal ConsumerManagerCore(
        ILogger<IPocKafkaPubSub> logger,
        ITimeProvider timeProvider,
        IDelayService delayService,
        IKafkaConsumerFactory consumerFactory,
        IKafkaProducerFactory producerFactory,
        IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        _delayService = delayService;
        _consumerConfiguration = consumerConfiguration;

        _lazyConsumer = CreateLazyConsumer(consumerFactory, consumerConfiguration);
        _lazyProducer = CreateLazyProducer(producerFactory, consumerConfiguration.ToProducerConfiguration());
    }
    public void ConfigureTopicsSubscription()
    {
        _logger.LogInformation("Setting up consumer subscriptions.");
        var topicConfigurations = _consumerConfiguration.ConsumerConfig.Topics;

        var (topicPartitionsToAssign, topicsToSubscribe) = DetermineSubscriptions(topicConfigurations);

        if (topicsToSubscribe.Count > 0)
            Subscribe(topicsToSubscribe);

        if (topicPartitionsToAssign.Count > 0)
            Assign(topicPartitionsToAssign);
    }
    private static (List<TopicPartition> TopicPartitionsToAssign, List<string> TopicsToSubscribe) DetermineSubscriptions(
        IEnumerable<PocTopicSettings> topicConfigurations)
    {
        var topicPartitionsToAssign = new List<TopicPartition>();
        var topicsToSubscribe = new List<string>();

        foreach (var topicConfig in topicConfigurations)
        {
            if (topicConfig.Partitions is { Length: > 0 })
            {
                topicPartitionsToAssign
                    .AddRange(topicConfig.Partitions.Select(partition => new TopicPartition(
                        topicConfig.Topic,
                        new Partition(partition))));
            }
            else
            {
                topicsToSubscribe.Add(topicConfig.Topic!);
            }
        }

        return (topicPartitionsToAssign, topicsToSubscribe);
    }

    public void Subscribe(IEnumerable<string> topics)
    {
        _lazyConsumer.Value.Subscribe(topics);
        _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", topics));
    }

    private void Assign(IEnumerable<TopicPartition> topicPartitions)
    {
        _lazyConsumer.Value.Assign(topicPartitions);
        _logger.LogInformation("Assigned to topics and partitions: {Assignments}",
            string.Join(", ", topicPartitions.Select(tp => $"{tp.Topic}:{tp.Partition.Value}")));
    }

    public ConsumeResult<TKey, TValue> ConsumeMessage(CancellationToken cancellationToken)
    {
        var consumeResult = _lazyConsumer.Value.Consume(cancellationToken);
        if (consumeResult is { Topic: not null, Message: not null })
        {
            _logger.LogInformation(
                "Consumed message. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}.",
                consumeResult.Topic,
                consumeResult.Partition,
                consumeResult.Offset,
                consumeResult.Message.Key);
        }

        return consumeResult;
    }

    public void Seek(TopicPartitionOffset topicPartitionOffset)
    {
        try
        {
            _lazyConsumer.Value.Seek(topicPartitionOffset);
            _logger.LogInformation(
                "Seeking to Topic: {Topic}, Partition: {Partition} e Offset: {Offset}",
                topicPartitionOffset.Topic,
                topicPartitionOffset.Partition.Value,
                topicPartitionOffset.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeking Topic: {Topic}, Partition: {Partition} e Offset: {Offset}.",
                topicPartitionOffset.Topic,
                topicPartitionOffset.Partition.Value,
                topicPartitionOffset.Offset);
        }
    }

    public async Task ProcessMessageAsync(
       ConsumeResult<TKey, TValue> consumeResult,
       Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
       CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing message from {Topic}, {Partition}, {Offset}.",
                consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

        if (consumeResult.IsPartitionEOF)
        {
            await ProcessEofAsync(consumeResult, cancellationToken);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        await ProcessMessageModeAsync(consumeResult, onMessageReceived, cancellationToken);
    }


    private async Task ProcessEofAsync(
        ConsumeResult<TKey, TValue> consumeResult,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("EOF for {Topic}, {Partition}, {Offset}.",
                 consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

        await WaitForAllTasksCompletion();

        if (_consumerConfiguration.ConsumerConfig.DelayPartitionEofMs > 0)
            await _delayService.Delay(_consumerConfiguration.ConsumerConfig.DelayPartitionEofMs, cancellationToken);
    }

    private async Task ProcessMessageModeAsync(
        ConsumeResult<TKey, TValue> consumeResult,
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {

        var topicPartition = consumeResult.TopicPartition;

        StartAndTrackMessageProcessing(topicPartition, consumeResult, onMessageReceived, cancellationToken);

        if (_consumerConfiguration.ConsumerConfig.IsConcurrentlyProcess)
            await ProcessMessagesConcurrentlyAsync();
        else
            await ProcessMessagesSequentiallyAsync(topicPartition);
    }

    private async Task ProcessMessagesSequentiallyAsync(TopicPartition topicPartition)
    {
        if (_partitionTasks.TryGetValue(topicPartition.ToString(), out var existingTask))
            await existingTask;
    }

    private async Task ProcessMessagesConcurrentlyAsync()
    {
        if (_partitionTasks.Count >= _consumerConfiguration.ConsumerConfig.MaxConcurrentMessages)
            await WaitForAllTasksCompletion();
    }

    private void StartAndTrackMessageProcessing(
        TopicPartition topicPartition,
        ConsumeResult<TKey, TValue> consumeResult,
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        var newTask = InvokeOnMessageReceivedAsync(consumeResult, onMessageReceived, cancellationToken);
        TrackPartitionTask(topicPartition, newTask, cancellationToken);
    }

    private void TrackPartitionTask(TopicPartition topicPartition, Task newTask, CancellationToken cancellationToken)
    {
        _partitionTasks[topicPartition.ToString()] = newTask.ContinueWith(task =>
        {
            HandleTaskCompletion(task, topicPartition);
        }, cancellationToken);
    }

    private void HandleTaskCompletion(Task task, TopicPartition topicPartition)
    {
        if (task.IsFaulted)
        {
            _logger.LogError(task.Exception, "Error processing message for partition {Partition}.", topicPartition.Partition);
        }

        _ = _partitionTasks.TryRemove(topicPartition.ToString(), out _);
    }

    private async Task WaitForAllTasksCompletion()
    {
        if (!_partitionTasks.IsEmpty)
        {
            await Task.WhenAll(_partitionTasks.Values);
            
            RemoveCompletedTasks();
        }
    }

    private void RemoveCompletedTasks()
    {
        var completedTasks = _partitionTasks.Where(p => p.Value.IsCompleted).Select(p => p.Key).ToList();
        foreach (var key in completedTasks)
        {
            _partitionTasks.TryRemove(key, out var _);
        }
    }

    private async Task InvokeOnMessageReceivedAsync(
        ConsumeResult<TKey, TValue> consumeResult,
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {

        var pocConsumeResult = PocConsumeResult<TKey, TValue>.Create(
            consumeResult,
            retryLimit: _consumerConfiguration.ConsumerConfig.RetryLimit);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await onMessageReceived(pocConsumeResult);

            stopwatch.Stop();

            _logger.LogDebug(
                 "Processed message from Topic: {Topic}, Partition: {Partition}, Offset: {Offset}," +
                " Key: {MessageKey}, ProcessingTime: {Elapsed}ms.",
                 consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key, stopwatch.ElapsedMilliseconds);

            await FinalizeMessageProcessingAsync(pocConsumeResult, consumeResult, cancellationToken);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Failed processing message from Topic: {Topic}, Partition: {Partition}, " +
                "Offset: {Offset}, Key: {MessageKey}, at {Timestamp}. ProcessingTime: {Elapsed}ms.",
                consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key,
                consumeResult.Message.Timestamp.UtcDateTime, stopwatch.ElapsedMilliseconds);

            await HandleFallbackAsync(pocConsumeResult, consumeResult, cancellationToken);
        }
    }

    private async Task FinalizeMessageProcessingAsync(
        PocConsumeResult<TKey, TValue> pocConsumeResult,
        ConsumeResult<TKey, TValue> consumeResult,
        CancellationToken cancellationToken)
    {
        if (pocConsumeResult.SkipRetryAndSendToDeadLetter)
        {
            await MoveToDeadLetterAsync(consumeResult, cancellationToken);
        }
        else if (pocConsumeResult.ShouldRetry)
        {
            await RetryMessageAsync(consumeResult, cancellationToken);
        }
        else
        {
            Commit(consumeResult);
        }
    }

    private async Task HandleFallbackAsync(
        PocConsumeResult<TKey, TValue> pocConsumeResult,
        ConsumeResult<TKey, TValue> consumeResult,
        CancellationToken cancellationToken)
    {
        if (pocConsumeResult.SkipRetryAndSendToDeadLetter)
            await MoveToDeadLetterAsync(consumeResult, cancellationToken);
        else
            await RetryMessageAsync(consumeResult, cancellationToken);
    }

    private async Task RetryMessageAsync(
        ConsumeResult<TKey, TValue> consumeResult,
        CancellationToken cancellationToken)
    {
        int retryCount = consumeResult.Message.Headers.GetHeaderAs<int>(ConsumerConstant.HEADER_NAME_RETRY_COUNT);

        _logger.LogInformation(
          "Retrying message from Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {MessageKey}, " +
          "RetryCount: {RetryCount}, MaxRetries: {MaxRetries}.",
          consumeResult.Topic, consumeResult.Partition, consumeResult.Offset,
          consumeResult.Message.Key, retryCount,
          _consumerConfiguration.ConsumerConfig.RetryLimit);

        while (retryCount < _consumerConfiguration.ConsumerConfig.RetryLimit)
        {
            retryCount++;

            UpdateHeadersForRetry(consumeResult.Message.Headers, retryCount);

            try
            {
                await SendToRetryTopicAsync(message: consumeResult.Message, cancellationToken);
                Commit(consumeResult);

                _logger.LogInformation(
                        "Retry {RetryCount} for message from Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {MessageKey} successful.",
                    retryCount, consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key);

                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                  "Retry {RetryCount} failed for message from Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {MessageKey}. Error: {Error}",
                  retryCount, consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key, ex.Message);
            }
        }

        if (retryCount >= _consumerConfiguration.ConsumerConfig.RetryLimit)
        {
            await MoveToDeadLetterAsync(consumeResult, cancellationToken);

            _logger.LogWarning(
            "Reached retry limit for message from Topic: {Topic}, Partition: {Partition}, Offset: {Offset}," +
            " Key: {MessageKey}. Moving to dead letter topic.",
            consumeResult.Topic, consumeResult.Partition, consumeResult.Offset, consumeResult.Message.Key);
        }
    }
    private void UpdateHeadersForRetry(Headers headers, int retryCount)
    {
        headers ??= [];
        headers.AddOrUpdate(ConsumerConstant.HEADER_NAME_RETRY_COUNT, BitConverter.GetBytes(retryCount));
        headers.AddOrUpdate(ConsumerConstant.HEADER_NAME_RETRY_AFTER,
            BitConverter.GetBytes(_timeProvider.UtcNow.AddMilliseconds(_consumerConfiguration.ConsumerConfig.RetryDelayMs).ToUnixTimeMilliseconds()));
    }
    private async Task MoveToDeadLetterAsync(
        ConsumeResult<TKey, TValue> consumeResult,
        CancellationToken cancellationToken)
    {
        await SendToDeadLetterTopicAsync(
            message: consumeResult.Message,
            cancellationToken);

        Commit(consumeResult);
    }
    private void Commit(ConsumeResult<TKey, TValue> consumeResult)
    {
        if (_consumerConfiguration.ConsumerConfig.EnableAutoCommit)
        {
            _logger.LogInformation("Auto commit enabled.");
            return;
        }

        _lazyConsumer.Value.Commit(consumeResult);
        _logger.LogInformation("Message commit successful. Key: {Key}, Offset: {Offset}",
            consumeResult.Message.Key, consumeResult.Offset);
    }

    private async Task SendToRetryTopicAsync(
        Message<TKey, TValue> message,
        CancellationToken cancellationToken)
    {
        string? topicRetry = _consumerConfiguration.ConsumerConfig.TopicRetry;
        if (string.IsNullOrWhiteSpace(topicRetry))
        {
            _logger.LogInformation("TopicRetry has not been configured.");
            return;
        }

        var deliveryResult = await ProduceAsync(
            topic: topicRetry,
            message: message,
            cancellationToken);

        _logger.LogInformation(
                "Message successfully published to TopicRetry: {TopicRetry}. Key: {Key}. Offset: {Offset}. Partition: {Partition}",
                topicRetry,
                deliveryResult.Message.Key,
                deliveryResult.Offset,
                deliveryResult.Partition);
    }

    private async Task SendToDeadLetterTopicAsync(
        Message<TKey, TValue> message,
        CancellationToken cancellationToken)
    {
        string? topicDeadLetter = _consumerConfiguration.ConsumerConfig.TopicDeadLetter;

        try
        {
            if (string.IsNullOrWhiteSpace(topicDeadLetter))
            {
                _logger.LogInformation("TopicDeadLetter has not been configured.");
                return;
            }

            var deliveryResult = await ProduceAsync(
              topic: topicDeadLetter,
              message: message,
              cancellationToken);

            _logger.LogInformation(
                    "Message successfully published to TopicDeadLetter: {TopicRetry}. Key: {Key}. Offset: {Offset}. Partition: {Partition}",
                    _consumerConfiguration.ConsumerConfig.TopicRetry,
                    deliveryResult.Message.Key,
                    deliveryResult.Offset,
                    deliveryResult.Partition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
               "An error occurred while sending message to dead letter topic: {TopicDeadLetter} exception message: {Message}.",
               topicDeadLetter, ex.Message);
        }
    }

    private async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topic,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken) =>
        await _lazyProducer.Value.ProduceAsync(topic, message, cancellationToken);

    private static Lazy<IProducer<TKey, TValue>> CreateLazyProducer(
        IKafkaProducerFactory producerFactory,
        IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        return new Lazy<IProducer<TKey, TValue>>(
                    () => producerFactory.CreateProducer(producerConfiguration),
                    LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private static Lazy<IConsumer<TKey, TValue>> CreateLazyConsumer(
        IKafkaConsumerFactory consumerFactory,
        IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        return new Lazy<IConsumer<TKey, TValue>>(
                    () => consumerFactory.CreateConsumer(consumerConfiguration),
                    LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing ConsumerManagerCore - {Name}.", _consumerConfiguration.ConsumerConfig.Name);

        if (_lazyConsumer.IsValueCreated)
        {
            _lazyConsumer.Value.Close();
            _lazyConsumer.Value.Dispose();
            _logger.LogInformation("Kafka consumer disposed.");
        }

        if (_lazyProducer.IsValueCreated)
        {
            _lazyProducer.Value.Dispose();
            _logger.LogInformation("Kafka producer retry disposed.");
        }

        await WaitForAllTasksCompletion();
    }
}
