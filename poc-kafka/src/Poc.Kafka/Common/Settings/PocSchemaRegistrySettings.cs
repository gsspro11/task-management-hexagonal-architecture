namespace Poc.Kafka.Common.Settings;

/// <summary>
/// Represents the settings for the Schema Registry.
/// </summary>
public class PocSchemaRegistrySettings
{
    /// <summary>
    /// Gets or sets the URL of the Schema Registry.
    /// </summary>
    public required string Url { get; init; }
    /// <summary>
    /// Gets or sets the username for authentication with the Schema Registry.
    /// </summary>
    public string? Username { get; init; }
    /// <summary>
    /// Gets or sets the password for authentication with the Schema Registry.
    /// </summary>
    public string? Password { get; init; }
}