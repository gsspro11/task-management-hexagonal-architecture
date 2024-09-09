using Confluent.Kafka;

namespace Poc.Kafka.Common.Settings;
/// <summary>
/// Configuration settings for a Kafka producer.
/// </summary>
public class PocProducerSettings
{
    /// <summary>
    /// Producer configuration name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Target topic for the producer.
    /// </summary>
    public required string Topic { get; init; }

    /// <summary>
    /// Acknowledgment strategy for message delivery.
    /// </summary>
    public Acks? Acks { get; init; }

    /// <summary>
    /// Enable sending API version request.
    /// </summary>
    public bool? ApiVersionRequest { get; init; }

    /// <summary>
    /// Max retries for message sending.
    /// </summary>
    public int? MessageSendMaxRetries { get; init; }

    /// <summary>
    /// Max number of in-flight messages.
    /// </summary>
    public int? MaxInFlight { get; init; }

    /// <summary>
    /// Optional transactional ID for exactly-once semantics.
    /// </summary>
    public string? TransactionalId { get; init; }

    /// <summary>
    /// Transaction timeout in milliseconds.
    /// </summary>
    public int? TransactionTimeoutMs { get; init; }

    /// <summary>
    /// Batch size in bytes for message batching.
    /// </summary>
    public int? BatchSize { get; init; }

    /// <summary>
    /// Number of messages to batch before sending.
    /// </summary>
    public int? BatchNumMessages { get; init; }

    /// <summary>
    /// Time to wait before sending a batch in milliseconds.
    /// </summary>
    public double? LingerMs { get; init; }
}