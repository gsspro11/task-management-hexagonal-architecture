
namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Represents settings for Kafka configurations, including clusters.
/// </summary>
public class PocKafkaSettings
{
    /// <summary>
    /// List of configurations for Kafka clusters.
    /// </summary>
    public required List<PocClusterSettings> Clusters { get; init; }
}