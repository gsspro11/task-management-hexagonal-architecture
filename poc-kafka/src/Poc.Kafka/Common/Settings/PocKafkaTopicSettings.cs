using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Defines the settings for a specific Kafka topic. This class is obsolete and PocKafkaSettings should be used instead.
/// </summary>
[ExcludeFromCodeCoverage]
[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed",
    Justification = "PocKafkaSettings introduces a unified approach to manage both broker and topic configurations," +
    " simplifying the setup process and enhancing compatibility with Kafka's latest features.")]
[Obsolete("Use PocKafkaSettings instead.")]

public class PocKafkaTopicSettings
{
    /// <summary>
    /// Indicates if the topic configuration should be ignored. Defaults to false.
    /// </summary>
    public bool? NoConfig { get; init; } = false;

    /// <summary>
    /// Consumer group ID.
    /// </summary>
    public string? GroupId { get; init; }

    /// <summary>
    /// Topic name for retries.
    /// </summary>
    public string? TopicRetry { get; init; }

    /// <summary>
    /// Topic name for dead-letter messages.
    /// </summary>
    public string? TopicDeadLetter { get; init; }

    /// <summary>
    /// Maximum number of retries for sending a message.
    /// </summary>
    public int? MessageSendMaxRetries { get; init; }

    /// <summary>
    /// Maximum number of messages that can be in flight simultaneously per partition.
    /// </summary>
    public int? MaxInFlight { get; init; }

    /// <summary>
    /// Timeout for consumer session in milliseconds.
    /// </summary>
    public int? SessionTimeoutMs { get; init; }

    /// <summary>
    /// Delay before considering the end of a partition in milliseconds.
    /// </summary>
    public int? DelayPartitionEofMs { get; init; }

    /// <summary>
    /// Limit for message retry attempts.
    /// </summary>
    public int? RetryLimit { get; init; }

    /// <summary>
    /// Limit for message retry attempts for day.
    /// </summary>
    public int? RetryDayLimit { get; init; }

    /// <summary>
    /// Delay between retry attempts in milliseconds.
    /// </summary>
    public int? RetryDelayMs { get; init; }

    /// <summary>
    /// Maximum number of concurrent messages that can be processed.
    /// </summary>
    public int? MaxConcurrentMessages { get; init; }
}