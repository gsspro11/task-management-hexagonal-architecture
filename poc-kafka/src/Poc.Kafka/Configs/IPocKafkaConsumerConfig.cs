using Poc.Kafka.Common.Settings;
using Confluent.Kafka;

namespace Poc.Kafka.Configs;

/// <summary>
/// Defines the configuration properties for a Kafka consumer, detailing aspects like topic subscription, session management,
/// and consumer behavior adjustments based on broker communication.
/// </summary>
public interface IPocKafkaConsumerConfig
{
    /// <summary>
    /// Unique identifier name for the consumer.
    /// </summary>
    string? Name { get; }
    /// <summary>
    /// Desired topics to consume from, used by the consumer group leader to allocate partitions.
    /// </summary>
    List<PocTopicSettings> Topics { get; }
    /// <summary>
    /// Client group ID string for group coordination and message distribution.
    /// </summary>
    string? GroupId { get; }
    /// <summary>
    /// Interval in milliseconds for emitting librdkafka statistics. A zero value disables this feature.
    /// Setting this provides insights into consumer performance and issues. Default: 0 (disabled). Importance: High.
    /// </summary>
    int? StatisticsIntervalMs { get; }
    /// <summary>
    /// Session timeout in milliseconds for consumer group failure detection.
    /// Shorter timeouts lead to quicker rebalance, longer timeouts may delay failure detection. Default: 45000. Importance: High.
    /// </summary>
    int? SessionTimeoutMs { get; }
    /// <summary>
    /// Heartbeat interval in milliseconds for group session keepalive. Default: 3000. Importance: Low.
    /// Correct documentation ensures this property is clearly understood.
    /// </summary>
    int? HeartbeatIntervalMs { get; }
    /// <summary>
    /// Maximum allowed time between calls to consume messages (e.g., rd_kafka_consumer_poll()) for high-level consumers.
    /// If this interval is exceeded, the consumer is considered failed, and the group will rebalance to reassign the partitions.
    /// Warning: Offset commits may not be possible at this point. Default: 300000. Importance: High.
    /// </summary>
    int? MaxPollIntervalMs { get; }
    /// <summary>
    /// Automatically commits offsets if enabled. Disabling impacts message reprocessing. Default: true.
    /// </summary>
    bool EnableAutoCommit { get; }
    /// <summary>
    /// Emit event when the consumer reaches the end of a partition. Default: false. Importance: Low.
    /// </summary>
    bool EnablePartitionEof { get; }
    /// <summary>
    /// Name of the retry topic for message reprocessing after an initial failure. Null if not used.
    /// </summary>
    string? TopicRetry { get; }
    /// <summary>
    /// Name of the dead letter topic for messages that can't be processed after retries. Null if not used.
    /// </summary>
    string? TopicDeadLetter { get; }
    /// <summary>
    /// Enables the activation of a consumer for the retry topic within the same process. When enabled, it initiates a separate consumer instance
    /// for handling messages from the retry topic, operating alongside the primary consumer. This setup allows for specialized processing of retry
    /// messages using a distinct consumer configuration, while maintaining the operational context within the same application process. Default: false.
    /// </summary>
    bool EnableRetryTopicConsumer { get; }
    /// <summary>
    /// Maximum retry attempts for message processing with retry topic subscription. Default: 3.
    /// </summary>
    int RetryLimit { get; }
    /// <summary>
    /// Delay in milliseconds between retry attempts, controlling retry frequency. Default: 0 (no delay).
    /// </summary>
    int RetryDelayMs { get; }
    /// <summary>
    /// Request broker's supported API versions to adjust functionality to available protocol features. Default: true. Importance: High.
    /// </summary>
    bool ApiVersionRequest { get; }
    /// <summary>
    /// Action to take when there is no initial offset in offset store or the desired
    /// offset is out of range: 'smallest','earliest' - automatically reset the offset
    /// to the smallest offset, 'largest','latest' - automatically reset the offset to
    /// the largest offset, 'error' - trigger an error (ERR__AUTO_OFFSET_RESET) which
    /// is retrieved by consuming messages and checking 'message->err'. default: largest
    /// importance: high
    /// </summary>
    AutoOffsetReset? AutoOffsetReset { get; }
    /// <summary>
    /// Defines a delay in milliseconds to be applied when an end-of-file (EOF) 
    /// event is emitted for a partition. This property sets a pause duration 
    /// to temporarily halt processing upon encountering an EOF event, allowing
    /// for a buffered handling of stream finalization or waiting for additional
    /// messages. The default value is 1000 milliseconds, introducing a one-second
    /// delay before proceeding after an EOF event.
    /// </summary>
    int DelayPartitionEofMs { get; }
    /// <summary>
    /// Maximum time the broker may wait to fill the Fetch response with fetch.min.bytes
    /// of messages. default: 500 importance: low
    /// </summary>
    int? FetchWaitMaxMs { get; }
    /// <summary>
    /// Minimum number of bytes the broker responds with. If fetch.wait.max.ms expires
    /// the accumulated data will be sent to the client regardless of this setting. default: 1 importance: low
    /// </summary>
    int? FetchMinBytes { get; }
    /// <summary>
    /// Maximum amount of data the broker shall return for a Fetch request. Messages
    /// are fetched in batches by the consumer and if the first message batch in the
    /// first non-empty partition of the Fetch request is larger than this value, then
    /// the message batch will still be returned to ensure the consumer can make progress.
    /// The maximum message batch size accepted by the broker is defined via `message.max.bytes`
    /// (broker config) or `max.message.bytes` (broker topic config). `fetch.max.bytes`
    /// is automatically adjusted upwards to be at least `message.max.bytes` (consumer
    /// config). default: 52428800 importance: medium
    /// </summary>
    int? FetchMaxBytes { get; }
    /// <summary>
    /// Initial maximum number of bytes per topic+partition to request when fetching
    /// messages from the broker. If the client encounters a message larger than this
    /// value it will gradually try to increase it until the entire message can be fetched.
    /// default: 1048576 importance: medium
    /// </summary>
    int? MaxPartitionFetchBytes { get; }
    /// <summary>
    /// The name of one or more partition assignment strategies.The elected group leader
    /// will use a strategy supported by all members of the group to assign partitions
    /// to group members. If there is more than one eligible strategy, preference is
    /// determined by the order of this list (strategies earlier in the list have higher
    /// priority). Cooperative and non-cooperative (eager) strategies must not be mixed.
    /// Available strategies: range, roundrobin, cooperative-sticky. default: range,roundrobin
    /// importance: medium
    /// </summary>
    PartitionAssignmentStrategy? PartitionAssignmentStrategy { get; }
    /// <summary>
    /// Gets the current limit on the number of messages the consumer can process simultaneously.
    /// A value of 0 indicates no limit, 1 ensures sequential processing, and values greater than 1 allow for concurrent processing of multiple messages, up to the specified limit.
    /// The default setting is 1, which is suitable for most scenarios, including when order of message processing is important.
    /// </summary>
    int MaxConcurrentMessages { get; }
    /// <summary>
    /// Sets the name associated with the consumer.
    /// </summary>
    /// <remarks>
    /// This method assigns a name to the consumer for identification purposes.
    /// </remarks>
    /// <param name="name">The name to associate with the consumer.</param>
    void SetName(string name);
    /// <summary>
    /// Sets the topic for the consumer to interact with. 
    /// This method clears any previously set topics and updates the
    /// list with the specified single topic. It is useful when the
    /// interaction needs to be limited or redirected to a specific topic. 
    /// </summary>
    /// <param name="topic">The name of the topic to set. 
    /// This will be the only topic in the list after this method is called.</param>
    void SetTopic(PocTopicSettings topic);
    /// <summary>
    /// Sets the topic for the consumer to interact with. This method clears any previously set topics
    /// and updates the list with the specified single topic. It is useful when the consumer's interaction
    /// needs to be limited or redirected to a specific topic.
    /// </summary>
    /// <param name="topics">The name of the topic to set. This will be the only topic in the list after
    /// this method is called. </param>
    void SetTopics(List<PocTopicSettings> topics);
    /// <summary>
    /// Assigns the Group ID for this consumer, identifying its consumer group for coordinated message consumption.
    /// </summary>
    /// <param name="groupId">The Group ID to assign.</param>
    void SetGroupId(string groupId);
    /// <summary>
    /// Sets the consumer's session timeout, used for failure detection within the consumer group. Shorter timeouts can lead to more frequent rebalancing, while longer timeouts may delay detection of failed consumers.
    /// </summary>
    /// <param name="sessionTimeoutMs">Session timeout in milliseconds.</param>
    void SetSessionTimeoutMs(int sessionTimeoutMs);
    /// <summary>
    /// Sets the heartbeat interval in milliseconds for the consumer. This interval is used to send heartbeats to the Kafka cluster to maintain 
    /// the consumer's membership in its consumer group. A suitable interval ensures the consumer remains active and connected to the group.
    /// </summary>
    /// <param name="heartbeatIntervalMs">The heartbeat interval in milliseconds.</param>
    void SetHeartbeatIntervalMs(int heartbeatIntervalMs);
    /// <summary>
    /// Sets the interval for how often statistics are emitted.
    /// </summary>
    /// <param name="statisticsIntervalMs">The interval, in milliseconds, between each statistics emission. 
    /// Setting this to zero disables the statistics. A positive value specifies the time interval.</param>
    void SetStatisticsIntervalMs(int statisticsIntervalMs);
    /// <summary>
    /// Sets the maximum polling interval (max.poll.interval.ms) for the consumer.
    /// </summary>
    /// <param name="maxPollIntervalMs">The value of the maximum polling interval in milliseconds. This value specifies the maximum duration
    /// that a poll call can block without invoking the Consume method. Exceeding this time without polling will signal to the broker
    /// that this consumer is failed, leading to a group rebalance and the reassignment of partitions to another consumer. Setting an
    /// appropriate value for this parameter helps balance long message processing with proper consumer failure detection.</param>
    void SetMaxPollIntervalMs(int maxPollIntervalMs);
    /// <summary>
    /// Sets the name of the retry topic. This specifies the topic to which messages should be sent for retrying after a failure.
    /// </summary>
    /// <param name="topicRetry">The name of the retry topic.</param>
    void SetTopicRetry(string topicRetry);
    /// <summary>
    /// Sets the name of the dead letter topic. This specifies the topic to which messages should be sent if they cannot be processed successfully after all retries.
    /// </summary>
    /// <param name="topicDeadLetter">The name of the dead letter topic.</param>
    void SetTopicDeadLetter(string topicDeadLetter);
    /// <summary>
    /// Activates a separate consumer instance for the retry topic within the same process. When this method is called, it configures the system to initiate 
    /// and manage a dedicated consumer for handling retry messages, ensuring that messages requiring reprocessing are managed with potentially distinct 
    /// consumer behaviors or configurations. This allows for specialized processing strategies for retry messages alongside the primary message consumption.
    /// </summary>
    void SetEnableRetryTopicConsumer();
    /// <summary>
    /// Sets whether the API version request feature is enabled. When enabled, this allows the client to check and use the API versions supported by the broker.
    /// </summary>
    /// <param name="apiVersionRequest">A boolean value indicating whether the API version request is enabled.</param>
    void SetApiVersionRequest(bool apiVersionRequest);
    /// <summary>
    /// Sets the auto offset reset behavior for the consumer. This setting determines how the consumer behaves when there is no initial 
    /// offset in Kafka or if the current offset does not exist anymore on the server (e.g., because the data has been deleted).
    /// </summary>
    /// <param name="autoOffsetReset">The auto offset reset behavior to set, which can be either earliest, latest, or none.</param>
    void SetAutoOffsetReset(AutoOffsetReset autoOffsetReset);
    /// <summary>
    /// Defines the limit of attempts to process a message. After exceeding this limit, it will no longer be retained.
    /// </summary>
    /// <param name="retryLimit">The maximum number of retry attempts.</param>
    void SetRetryLimit(int retryLimit);
    /// <summary>
    /// Sets the delay in milliseconds between retry attempts for processing messages. 
    /// This delay allows the system to pause between retries, potentially resolving temporary issues and improving the 
    /// chances of successful message processing. A value of 0 means no delay between retries.
    /// </summary>
    /// <param name="retryDelayMs">The delay in milliseconds between retries. A value of 0 indicates immediate retry without delay.</param>
    void SetRetryDelayMs(int retryDelayMs);
    /// <summary>
    /// Enables or disables automatic offset commit, affecting how and when message processing offsets are stored.
    /// </summary>
    void SetEnableAutoCommit();
    /// <summary>
    /// Configure the delay, in milliseconds, to be applied when the consumer reaches the end of a partition.
    /// </summary>
    /// <param name="delayPartitionEofMs">The amount of seconds to delay.</param>
    void SetDelayPartitionEofMs(int delayPartitionEofMs);
    /// <summary>
    /// Sets the maximum wait time in milliseconds for the consumer to fetch data from the server.
    /// </summary>
    /// <param name="fetchWaitMaxMs">The maximum wait time in milliseconds for a fetch operation.</param>
    void SetFetchWaitMaxMs(int fetchWaitMaxMs);
    /// <summary>
    /// Sets the minimum amount of data that the consumer should expect to receive in a single fetch request.
    /// </summary>
    /// <param name="fetchMinBytes">The minimum size in bytes for a single fetch request.</param>
    void SetFetchMinBytes(int fetchMinBytes);
    /// <summary>
    /// Sets the maximum amount of data the consumer can fetch in a single request.
    /// </summary>
    /// <param name="fetchMaxBytes">The maximum size in bytes for a single fetch request.</param>
    void SetFetchMaxBytes(int fetchMaxBytes);
    /// <summary>
    /// Sets the maximum amount of data the consumer can fetch from a single partition.
    /// </summary>
    /// <param name="maxPartitionFetchBytes">The maximum size in bytes for a single fetch request per partition.</param>
    void SetMaxPartitionFetchBytes(int maxPartitionFetchBytes);
    /// <summary>
    /// Configures the partition assignment strategy to be used by the consumer.
    /// The elected group leader will use a strategy supported by all members of the group to assign partitions to group members.
    /// Default: range,roundrobin. Importance: Medium.
    /// </summary>
    /// <param name="partitionAssignmentStrategy">The partition assignment strategy or strategies to be used, specified in order of preference.</param>
    void SetPartitionAssignmentStrategy(PartitionAssignmentStrategy partitionAssignmentStrategy);
    /// <summary>
    /// Sets the maximum number of messages the consumer can process concurrently. Adjusts workload management.
    /// Use caution with high values to avoid resource overuse. Default is 1, ensuring sequential processing.
    /// </summary>
    /// <param name="maxConcurrentMessages">Maximum number of concurrent messages. Default value is 1.</param>
    public void SetMaxConcurrentMessages(int maxConcurrentMessages);
}