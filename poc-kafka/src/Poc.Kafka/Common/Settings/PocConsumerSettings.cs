namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Configuration settings for a Kafka consumer.
/// </summary>
public class PocConsumerSettings
{
    /// <summary>
    /// Name of the consumer configuration.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// Topics subscribed to by the consumer.
    /// </summary>
    public required List<PocTopicSettings> Topics { get; init; }
    /// <summary>
    /// Consumer group ID.
    /// </summary>
    public required string GroupId { get; init; }
    /// <summary>
    /// Enable sending API version request.
    /// </summary>
    public bool? ApiVersionRequest { get; init; }
    /// <summary>
    /// Session timeout in milliseconds.
    /// </summary>
    public int? SessionTimeoutMs { get; init; }
    /// <summary>
    /// Heartbeat interval in milliseconds.
    /// </summary>
    public int? HeartbeatIntervalMs { get; init; }
    /// <summary>
    /// Delay before considering a partition end-of-file.
    /// </summary>
    public int? DelayPartitionEofMs { get; init; }
    /// <summary>
    /// Retry limit for message consumption.
    /// </summary>
    public int? RetryLimit { get; init; }
    /// <summary>
    /// Delay between retries in milliseconds.
    /// </summary>
    public int? RetryDelayMs { get; init; }
    /// <summary>
    /// Optional retry topic.
    /// </summary>
    public string? TopicRetry { get; init; }
    /// <summary>
    /// Optional dead-letter topic.
    /// </summary>
    public string? TopicDeadLetter { get; init; }
    /// <summary>
    /// Max wait time for fetching messages in milliseconds.
    /// </summary>
    public int? FetchWaitMaxMs { get; init; }
    /// <summary>
    /// Min data to fetch in bytes.
    /// </summary>
    public int? FetchMinBytes { get; init; }
    /// <summary>
    /// Max data to fetch in bytes.
    /// </summary>
    public int? FetchMaxBytes { get; init; }
    /// <summary>
    /// Max data per partition to fetch in bytes.
    /// </summary>
    public int? MaxPartitionFetchBytes { get; init; }
    /// <summary>
    /// Max data per partition to fetch in bytes.
    /// </summary>
    public int? MaxPollIntervalMs { get; init; }
    /// <summary>
    /// Maximum number of messages that can be concurrently processed.
    /// </summary>
    public int? MaxConcurrentMessages { get; init; }
}