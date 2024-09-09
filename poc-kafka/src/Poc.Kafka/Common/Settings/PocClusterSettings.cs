namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Configuration settings for an individual Kafka cluster.
/// </summary>
public class PocClusterSettings
{
    /// <summary>
    /// Name of the Kafka cluster.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// Bootstrap servers for the Kafka cluster.
    /// </summary>
    public required string BootstrapServers { get; init; }
    /// <summary>
    /// Optional SASL authentication username.
    /// </summary>
    public string? Username { get; init; }
    /// <summary>
    /// Optional SASL authentication password.
    /// </summary>
    public string? Password { get; init; }
    /// <summary>
    /// Schema Registry settings for the cluster.
    /// </summary>
    public PocSchemaRegistrySettings? SchemaRegistrySettings { get; init; }
    /// <summary>
    /// Consumer configurations for the cluster.
    /// </summary>
    public required List<PocConsumerSettings> Consumers { get; init; }
    /// <summary>
    /// Producer configurations for the cluster.
    /// </summary>
    public required List<PocProducerSettings> Producers { get; init; }
}
