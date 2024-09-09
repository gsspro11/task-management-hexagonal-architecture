using Confluent.Kafka;

namespace Poc.Kafka.Configs;

/// <summary>
/// Defines the configuration properties for a Kafka producer, including details on message delivery semantics, broker communication adjustments, and batching strategies.
/// </summary>
public interface IPocKafkaProducerConfig
{
    /// <summary>
    /// Unique identifier name for the producer. This name can be used for logging, monitoring, and troubleshooting purposes.
    /// </summary>
    string? Name { get; }
    /// <summary>
    /// The target topic for message production. Specifies where messages produced by this producer will be sent.
    /// </summary>
    string? Topic { get; }
    /// <summary>
    /// Enables exactly-once message production semantics. When enabled, ensures messages are produced exactly once to a particular topic partition in the original order. 
    /// Note: Enabling idempotence adjusts several other configurations automatically to ensure compatibility.
    /// Default: false. Importance: High.
    /// </summary>
    bool EnableIdempotence { get; }
    /// <summary>
    /// Enables querying of broker's supported API versions to adjust the producer's functionality accordingly. 
    /// If disabled or if the request fails, the producer uses the fallback version specified by `broker.version.fallback`.
    /// Default: true. Importance: High.
    /// </summary>
    bool ApiVersionRequest { get; }
    /// <summary>
    /// Defines the maximum number of attempts to resend a message that fails to send. 
    /// Note: Setting `enable.idempotence` to true may influence retry behavior.
    /// Default: INT32_MAX. Importance: High.
    /// </summary>
    int? MessageSendMaxRetries { get; }
    /// <summary>
    /// Specifies the acknowledgment level required from the broker to consider a message as successfully sent.
    /// Varies from 0 (no acknowledgment) to all (acknowledgment from all in-sync replicas).
    /// </summary>
    Acks? Acks { get; }
    /// <summary>
    /// Limits the number of in-flight requests per broker connection to control load and manage message delivery
    /// throughput. Default: 1000000. Importance: Low.
    /// </summary>
    int? MaxInFlight { get; }
    /// <summary>
    /// Identifies the transactional producer across process restarts, enabling transactional message delivery
    /// with strict ordering and atomicity guarantees. Importance: High.
    /// </summary>
    string? TransactionalId { get; }
    /// <summary>
    /// Sets the transaction timeout, coordinating the maximum allowed time for a transaction's completion.
    /// Default: 60000. Importance: Medium.
    /// </summary>
    int? TransactionTimeoutMs { get; }
    /// <summary>
    /// Configures the maximum batch size for message sets before they are sent to the broker.
    /// Default: 1000000. Importance: Medium.
    /// </summary>
    int? BatchSize { get; }
    /// <summary>
    /// Determines the maximum number of messages collected into a batch before sending.
    /// Default: 10000. Importance: Medium.
    /// </summary>
    int? BatchNumMessages { get; }
    /// <summary>
    /// Sets the delay time to wait for additional messages before sending a batch to the broker.
    /// Default: 5. Importance: High.
    /// </summary>
    double? LingerMs { get; }
    /// <summary>
    /// Assigns a name to the producer for identification purposes.
    /// </summary>
    /// <remarks>
    /// This method sets the name associated with the producer, which serves as an identifier.
    /// </remarks>
    /// <param name="name">The name to be associated with the producer.</param>
    void SetName(string name);
    /// <summary>
    /// Sets the topic to which messages will be produced by the Kafka Producer.
    /// </summary>
    /// <param name="topic">The name of the topic.</param>
    void SetTopic(string topic);
    /// <summary>
    /// Enables idempotence for the Kafka producer. When idempotence is enabled, the producer ensures that messages 
    /// are delivered exactly once to a particular topic partition during the lifetime of a single producer instance.
    /// This is achieved by setting 'EnableIdempotence' to true, limiting 'MaxInFlight' to 5, setting 
    /// 'MessageSendMaxRetries' to 3, and ensuring that acknowledgments ('Acks') are received from all replicas 
    /// with 'Acks.All'. These default settings help in preventing message duplication and ensuring message delivery 
    /// reliability.
    /// </summary>
    void SetIdempotenceEnabled();
    /// <summary>
    /// Sets the 'ApiVersionRequest' flag for the Kafka client. This flag determines whether the client will request 
    /// API version information from the brokers. Setting this to true enables the client to automatically adjust 
    /// its usage of Kafka's API to match the version available on the brokers. This can be particularly useful for 
    /// compatibility across different versions of Kafka brokers.
    /// </summary>
    /// <param name="apiVersionRequest">Boolean flag to enable or disable API version requests.</param>
    void SetApiVersionRequest(bool apiVersionRequest);
    /// <summary>
    /// Sets the maximum number of message sending retries for the Kafka producer. This setting determines how many 
    /// times the producer will attempt to resend a message that failed to send initially. A higher number of retries 
    /// can increase the likelihood of successful message delivery but may result in higher latency and network traffic.
    /// </summary>
    /// <param name="messageSendMaxRetries">The maximum number of retries for message sending.</param>
    void SetMessageSendMaxRetries(int messageSendMaxRetries);
    /// <summary>
    /// Configures the acknowledgment level ('Acks') for messages sent by the Kafka producer. This setting determines 
    /// how many acknowledgments the producer requires from the brokers to consider a message delivery successful. 
    /// Different settings (e.g., 'None', 'Leader', 'All') offer trade-offs between performance and data durability.
    /// </summary>
    /// <param name="acks">The acknowledgment level to be used for message delivery.</param>
    void SetAcks(Acks acks);
    /// <summary>
    /// Sets the maximum number of messages that can be sent in parallel ('MaxInFlight') by the Kafka producer. 
    /// This setting controls the number of messages that can be in-flight (sent but not yet acknowledged) at any 
    /// given time. Lower values can lead to increased reliability, while higher values can improve throughput.
    /// </summary>
    /// <param name="maxInFlight">The maximum number of in-flight messages allowed.</param>
    void SetMaxInFlight(int maxInFlight);
    /// <summary>
    /// Sets the transactional identifier for the Kafka producer. This identifier is used to coordinate transactions
    /// across multiple messages, ensuring either all messages in a transaction are committed or none are. The method
    /// appends a GUID to the provided transactionalId, ensuring a unique identifier across different instances or
    /// application runs, which is essential for transaction management and recovery.
    /// Note: Use this method only if you intend to use the SendBatchAtomic method.
    /// </summary>
    /// <param name="transactionalId">The base transactional identifier to which a GUID will be appended.</param>
    void SetTransactionalId(string transactionalId);
    /// <summary>
    /// Sets the transaction timeout in milliseconds for the Kafka producer's transactions. 
    /// This timeout defines the maximum duration the transaction coordinator will wait for 
    /// a transaction status update from the producer before proactively aborting the transaction. 
    /// Adjusting this timeout also impacts `message.timeout.ms` and `socket.timeout.ms` settings, 
    /// ensuring they do not exceed the specified transaction timeout. The `socket.timeout.ms` 
    /// setting must be at least 100ms less than the `transaction.timeout.ms`. If the provided 
    /// transaction timeout exceeds the broker's `transaction.max.timeout.ms` setting, the 
    /// initialization of transactions will fail with an error. This value also serves as the 
    /// default timeout for transactional API methods if no specific timeout is provided.
    /// </summary>
    /// <param name="transactionTimeoutMs">The transaction timeout in milliseconds. 
    /// This value should be within the limits defined by the broker configuration.</param>
    void SetTransactionTimeoutMs(int transactionTimeoutMs);
    /// <summary>
    /// Sets the batch size for message processing. This determines the maximum number of messages to be processed
    /// in a single batch. Adjusting this value can impact the throughput and efficiency of message processing.
    /// </summary>
    /// <param name="batchSize">The maximum number of messages to process in a single batch.</param>
    void SetBatchSize(int batchSize);
    /// <summary>
    /// Sets the number of messages to accumulate before sending a batch. This setting controls the number of
    /// messages the producer will batch together before sending them to the broker, affecting throughput and latency.
    /// </summary>
    /// <param name="batchNumMessages">The number of messages to batch together for sending.</param>
    void SetBatchNumMessages(int batchNumMessages);
    /// <summary>
    /// Sets the maximum amount of time, in milliseconds, to wait for additional messages before sending a batch.
    /// This 'linger' time can help improve throughput and efficiency by batching smaller messages together,
    /// but may introduce additional latency.
    /// </summary>
    /// <param name="lingerMs">The linger time in milliseconds.</param>
    void SetLingerMs(double lingerMs);
}