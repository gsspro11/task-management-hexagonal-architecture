namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Settings for topics within a consumer configuration.
/// </summary>
public class PocTopicSettings
{
    /// <summary>
    /// Topic name.
    /// </summary>
    public required string Topic { get; init; }

    /// <summary>
    /// Specific partitions to consume.
    /// </summary>
    public int[] Partitions { get; init; } = [];
}