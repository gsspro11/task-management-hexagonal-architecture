namespace Poc.Kafka.Configs;

/// <summary>
/// Provides a base configuration for Kafka credentials, encapsulating the management of username and password used for authentication.
/// </summary>
public abstract class PocKafkaCredentialsConfig
{
    /// <summary>
    /// Indicates whether the credentials have been provided. This is useful for conditional logic based on the presence of credentials.
    /// </summary>
    public bool IsCredentialsProvided { get; private set; } = false;
    internal string? Username { get; private set; }
    internal string? Password { get; private set; }
    internal void SetCredentials(string username, string password)
    {
        Username = username;
        Password = password;
        IsCredentialsProvided = true;
    }
}
