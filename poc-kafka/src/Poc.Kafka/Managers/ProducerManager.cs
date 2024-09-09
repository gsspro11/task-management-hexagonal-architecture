using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.Managers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]
internal sealed class ProducerManager<TKey, TValue> : IProducerManager<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly IProducerConfiguration<TKey, TValue> _producerConfiguration;
    private readonly Lazy<IProducer<TKey, TValue>> _lazyProducer;

    internal ProducerManager(
        ILogger<IPocKafkaPubSub> logger,
        IKafkaProducerFactory producerFactory,
        IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        _logger = logger;
        _producerConfiguration = producerConfiguration;
        _lazyProducer = CreateLazyProducer(producerFactory, producerConfiguration);
    }

    public void Flush(TimeSpan timeout) =>
        _lazyProducer.Value.Flush(timeout);
    public void InitTransactions(TimeSpan timeout) =>
        _lazyProducer.Value.InitTransactions(timeout);

    public void BeginTransaction() =>
        _lazyProducer.Value.BeginTransaction();

    public void CommitTransaction() =>
        _lazyProducer.Value.CommitTransaction();

    public void AbortTransaction() =>
        _lazyProducer.Value.AbortTransaction();

    public async Task<DeliveryResult<TKey, TValue>> SendMessageAsync(
        Message<TKey, TValue> message,
        string? topic = null,
        CancellationToken cancellationToken = default)
    {
        topic ??= _producerConfiguration.ProducerConfig.Topic;

        var deliveryResult = await _lazyProducer.Value.ProduceAsync(
            topic,
            message,
            cancellationToken);

        _logger.LogInformation(
                "Message delivered to topic: {Topic} key: {Key} value: {Value}. Offset: {Offset}",
                deliveryResult.Topic,
                deliveryResult.Message.Key,
                deliveryResult.Message.Value,
                deliveryResult.Offset);

        return deliveryResult;
    }

    public void SendMessage(
      Message<TKey, TValue> message,
      string? topic = null,
      Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        topic ??= _producerConfiguration.ProducerConfig.Topic;

        _lazyProducer.Value.Produce(
            topic,
            message,
            deliveryHandler);
    }

    public void SendMessage(
        Message<TKey, TValue> message,
        Action<Message<TKey, TValue>, DeliveryReport<TKey, TValue>> deliveryHandler,
        string? topic = null) =>
        SendMessage(message, topic, deliveryReport => deliveryHandler(message, deliveryReport));

    private static Lazy<IProducer<TKey, TValue>> CreateLazyProducer(
        IKafkaProducerFactory producerFactory,
        IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        return new Lazy<IProducer<TKey, TValue>>(
                    () => producerFactory.CreateProducer(producerConfiguration),
                    LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing {TypeName}.", nameof(ProducerManager<TKey, TValue>));

        if (_lazyProducer.IsValueCreated)
        {
            _lazyProducer.Value.Flush();
            _lazyProducer.Value.Dispose();
            _logger.LogInformation("Kafka producer disposed.");
        }
    }
}
