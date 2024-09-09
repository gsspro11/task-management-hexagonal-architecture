namespace Poc.Kafka.Configs;

/// <summary>
/// Provides configuration settings for the Kafka Schema Registry, focusing on URL configuration and the option to leverage Kafka broker credentials for authentication. 
/// This interface facilitates the integration with Schema Registry by allowing straightforward configuration of connection details and authentication mechanisms.
/// </summary>
public interface IPocKafkaSchemaRegistryConfig
{
    /// <summary>
    /// The URL of the Schema Registry. This is a crucial setting that specifies the endpoint for schema management and retrieval.
    /// </summary>
    string? Url { get; }
    /// <summary>
    /// Sets the URL for the Schema Registry. This URL points to the Schema Registry service, which is used to store and retrieve Avro schemas for Kafka messages.
    /// </summary>
    /// <param name="url">The complete URL of the Schema Registry, including the protocol (http or https) and port number, if applicable.</param>
    void SetUrl(string url);
    /// <summary>
    /// Enables the usage of Kafka broker credentials for authenticating with the Schema Registry. This method configures the Schema Registry client to authenticate using the same credentials as the Kafka broker, facilitating environments where Schema Registry and Kafka brokers share the same authentication mechanism. 
    /// Note: This should only be used if the Schema Registry is configured to validate credentials against the Kafka broker's authentication settings.
    /// </summary>
    void UseBrokerCredentials();
}