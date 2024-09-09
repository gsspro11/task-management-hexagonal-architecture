using Poc.Kafka.Results;
using Confluent.Kafka;

namespace Poc.Kafka.PubSub;

/// <summary>
/// Defines a contract for publishing messages to Kafka topics in batches. Each message is identified by a key of type <typeparamref name="TKey"/> 
/// and contains a value of type <typeparamref name="TValue"/>. This interface facilitates efficient transmission of structured data to specific Kafka topics, 
/// allowing for data distribution and partitioning based on the message key. It supports batch operations and atomicity for message delivery where applicable.
/// If the topic is not specified for any method, the default topic from the producer's configuration will be used.
/// </summary>
/// <typeparam name="TKey">The type of the key used to partition messages within the Kafka topic, aiding in efficient data organization and retrieval.</typeparam>
/// <typeparam name="TValue">The type of the message value to be published, enabling the transmission of diverse data payloads to the Kafka infrastructure.</typeparam>
public interface IPocKafkaBatchPub<TKey, TValue>
{
    /// <summary>
    /// Asynchronously sends a batch of Kafka messages to the specified topic, awaiting delivery confirmation for each message. 
    /// Utilizes the asynchronous 'ProduceAsync' method of the Kafka producer to ensure reliable feedback on delivery status. 
    /// An optional batch ID can be provided for logging and tracking, enhancing traceability.
    /// </summary>
    /// <param name="messages">The collection of Kafka messages to be sent, each containing a key and value.</param>
    /// <param name="topic">The Kafka topic to which the messages will be sent. If not specified, the default topic from the producer's configuration is used.</param>
    /// <param name="batchId">Optional identifier for the batch, used for logging and tracking.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>A task representing the asynchronous send operation, with detailed results including successes and failures.</returns>
    Task<BatchSendResult<TKey, TValue>> SendBatchAsync(
        IEnumerable<Message<TKey, TValue>> messages,
        string? topic = null,
        Guid? batchId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a batch of Kafka messages, processing each message independently for optimized sending. 
    /// Does not ensure atomicity. Successes and failures are recorded individually, and a flush operation ensures delivery.
    /// An optional batch ID enhances tracking and logging of the operation.
    /// </summary>
    /// <param name="messages">The collection of Kafka messages for the batch operation.</param>
    /// <param name="topic">The Kafka topic to which the messages will be sent. If not specified, the default topic from the producer's configuration is used.</param>
    /// <param name="timeout">Optional flush operation timeout, with a default of 30 seconds.</param>
    /// <param name="batchId">Optional identifier for the batch, for enhanced tracking and logging.</param>
    /// <returns>Results of the batch send operation, including detailed successes and failures.</returns>
    BatchSendResult<TKey, TValue> SendBatch(
        IEnumerable<Message<TKey, TValue>> messages,
        string? topic = null,
        TimeSpan? timeout = null,
        Guid? batchId = null);

    /// <summary>
    /// Sends a batch of Kafka messages within a transaction to ensure atomicity: either all messages are sent successfully, or none are sent in the event of a failure. 
    /// Operates within a Kafka transaction to provide consistency and reliability. 
    /// An optional batch ID supports detailed tracking and logging of the transactional operation.
    /// </summary>
    /// <param name="messages">The collection of Kafka messages to be sent.</param>
    /// <param name="topic">The Kafka topic to which the messages will be sent as part of the transaction. If not specified, the default topic from the producer's configuration is used.</param>
    /// <param name="timeout">Optional operation timeout, with a sensible default applied.</param>
    /// <param name="batchId">Optional identifier for the transactional batch, aiding in operation tracking and logging.</param>
    /// <returns>Boolean indicating the success (true) or failure (false) of the transactional batch send operation.</returns>
    bool SendBatchAtomic(
       IEnumerable<Message<TKey, TValue>> messages,
       string? topic = null,
       TimeSpan? timeout = null,
       Guid? batchId = null);
}
