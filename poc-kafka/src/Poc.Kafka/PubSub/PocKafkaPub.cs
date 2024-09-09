using Poc.Kafka.Common;
using Poc.Kafka.Common.Exceptions;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.PubSub;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]

internal sealed partial class PocKafkaPub<TKey, TValue> : IPocKafkaPub<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly IProducerManager<TKey, TValue> _producerManager;
    private readonly IProducerConfiguration<TKey, TValue> _producerConfiguration;
    private bool disposedValue;

    internal PocKafkaPub(
        ILogger<IPocKafkaPubSub> logger,
        IProducerManager<TKey, TValue> producerManager,
        IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        _logger = logger;
        _producerManager = producerManager;
        _producerConfiguration = producerConfiguration;
    }

    public void Send(
        TValue value,
        TKey? key = default,
        Headers? headers = null,
        string? topic = null,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        var message = KafkaMessageFactory.CreateKafkaMessage(value, key, headers);
        SendInternal(topic, deliveryHandler, message);
    }

    public void Send(
       Message<TKey, TValue> message,
       string? topic = null,
       Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null) =>
        SendInternal(topic, deliveryHandler, message);

    public async Task<DeliveryResult<TKey, TValue>> SendAsync(
        TValue value,
        TKey? key = default,
        Headers? headers = default,
        string? topic = null,
        CancellationToken cancellationToken = default)
    {
        var message = KafkaMessageFactory.CreateKafkaMessage(value, key, headers);
        return await _producerManager.SendMessageAsync(message, topic, cancellationToken);
    }

    public async Task<DeliveryResult<TKey, TValue>> SendAsync(
        Message<TKey, TValue> message,
        string? topic = null,
        CancellationToken cancellationToken = default) =>
        await _producerManager.SendMessageAsync(message, topic, cancellationToken);

    private void SendDeliveryHandlerDelegate(
        DeliveryReport<TKey, TValue> deliveryReport)
    {
        if (deliveryReport.Error.IsError)
        {
            LogDeliveryHandlerError(deliveryReport);
            throw new KafkaDeliveryException(deliveryReport.Error.GetErrorFormatted());
        }
        else
            LogDeliveryHandlerSuccess(deliveryReport);
    }

    private void SendInternal(string? topic, Action<DeliveryReport<TKey, TValue>>? deliveryHandler, Message<TKey, TValue> message)
    {
        Action<DeliveryReport<TKey, TValue>> actualDeliveryHandler = deliveryHandler
            ?? (deliveryReport => SendDeliveryHandlerDelegate(deliveryReport));

        _producerManager.SendMessage(message, topic, deliveryHandler: actualDeliveryHandler);
    }

    private void LogDeliveryHandlerSuccess(DeliveryReport<TKey, TValue> deliveryReport)
    {
        _logger.LogInformation(
            "Message delivered to topic: {Topic} key: {Key} value: {Value}. Offset: {Offset}",
            deliveryReport.Topic,
            deliveryReport.Message.Key,
            deliveryReport.Message.Value,
            deliveryReport.Offset);
    }

    private void LogDeliveryHandlerError(DeliveryReport<TKey, TValue> deliveryReport)
    {
        _logger.LogError(
            "Message delivery failed for topic: {Topic} key: {Key} value: {Value}. Error: {Error}",
            deliveryReport.Topic,
            deliveryReport.Message.Key,
            deliveryReport.Message.Value,
            deliveryReport.Error.Reason);
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _logger.LogInformation("Disposing PocKafkaPub - {Name}.", _producerConfiguration.ProducerConfig.Name);
                _producerManager.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
