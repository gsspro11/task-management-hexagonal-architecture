using Confluent.Kafka;

namespace Poc.Kafka.PubSub;

/// <summary>
/// Define a contract for publishing messages to a Kafka topic, where each message consists of a key of type <typeparamref name="TKey"/> and a value of type <typeparamref name="TValue"/>.
/// This interface enables precise control over message delivery to Kafka topics, supporting both synchronous and asynchronous operations, with flexibility in message construction and delivery options.
/// </summary>
/// <typeparam name="TKey">The type of the key used to partition messages within the Kafka topic.</typeparam>
/// <typeparam name="TValue">The type of the message value to be published to the Kafka topic.</typeparam>
public interface IPocKafkaPub<TKey, TValue> : IPocKafkaBatchPub<TKey, TValue>, IDisposable
{
    /// <summary>
    /// Sends a message to the specified Kafka topic. Allows specifying a key, value, and optional headers. 
    /// This method supports custom delivery handlers for processing delivery reports.
    /// If the topic is not specified, the default topic from the producer's configuration is used.
    /// </summary>
    /// <param name="value">The value of the message.</param>
    /// <param name="key">The message key for partitioning. Can be null.</param>
    /// <param name="headers">Optional headers for additional metadata. Defaults to none if not specified.</param>
    /// <param name="topic">The target Kafka topic. If not specified, the default topic from the producer's configuration will be used.</param>
    /// <param name="deliveryHandler">Optional custom handler for the delivery report. Uses a default logger if not specified.</param>
    void Send(
       TValue value,
       TKey? key = default,
       Headers? headers = null,
       string? topic = null,
       Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null);

    /// <summary>
    /// Sends a pre-constructed Kafka message to the specified topic. This overload allows for sending a message that may already include key, value, and headers.
    /// If the topic is not specified, the default topic from the producer's configuration is used.
    /// </summary>
    /// <param name="message">The pre-constructed Kafka message.</param>
    /// <param name="topic">The target Kafka topic. If not specified, the default topic from the producer's configuration will be used.</param>
    /// <param name="deliveryHandler">Optional custom handler for the delivery report. Uses a default logger if not specified.</param>
    void Send(
       Message<TKey, TValue> message,
       string? topic = null,
       Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null);

    /// <summary>
    /// Asynchronously sends a message to the specified Kafka topic. Similar to the synchronous Send method, but awaits the delivery before proceeding.
    /// If the topic is not specified, the default topic from the producer's configuration is used.
    /// </summary>
    /// <param name="value">The value of the message.</param>
    /// <param name="key">The message key for partitioning. Can be null.</param>
    /// <param name="headers">Optional headers for additional metadata. Defaults to none if not specified.</param>
    /// <param name="topic">The target Kafka topic. If not specified, the default topic from the producer's configuration will be used.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a delivery report indicating the result.</returns>
    Task<DeliveryResult<TKey, TValue>> SendAsync(
         TValue value,
         TKey? key = default,
         Headers? headers = null,
         string? topic = null,
         CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously sends a pre-constructed Kafka message to the specified topic. This overload is useful for messages that already include all necessary details.
    /// If the topic is not specified, the default topic from the producer's configuration is used.
    /// </summary>
    /// <param name="message">The pre-constructed Kafka message.</param>
    /// <param name="topic">The target Kafka topic. If not specified, the default topic from the producer's configuration will be used.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a delivery report indicating the result.</returns>
    Task<DeliveryResult<TKey, TValue>> SendAsync(
      Message<TKey, TValue> message,
      string? topic = null,
      CancellationToken cancellationToken = default);
}