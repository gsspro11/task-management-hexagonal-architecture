using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Represents the settings for a Kafka broker. This class is obsolete and PocKafkaSettings should be used instead.
/// </summary>
[ExcludeFromCodeCoverage]
[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed",
    Justification = "PocKafkaSettings introduces a unified approach to manage both broker and topic configurations," +
    " simplifying the setup process and enhancing compatibility with Kafka's latest features.")]
[Obsolete("Use PocKafkaSettings instead.")]
public class PocKafkaBrokerSettings
{
    /// <summary>
    /// The bootstrap servers for Kafka connection.
    /// </summary>
    public required string BootstrapServer { get; init; }

    /// <summary>
    /// Optional username for authentication.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Optional password for authentication.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    /// Dictionary of topic settings keyed by topic name.
    /// </summary>
    public required Dictionary<string, PocKafkaTopicSettings> Topics { get; init; }
}