using Poc.Kafka.Common;
using Poc.Kafka.Configs;
using Poc.Kafka.Configurations;
using Confluent.SchemaRegistry;

namespace Poc.Kafka.Configurators;

/// <summary>
/// Enables fluent configuration of Kafka cluster settings, facilitating the setup of bootstrap servers,
/// authentication, topic creation, and the integration of producers and consumers with customized configurations.
/// </summary>
public interface IClusterConfigurator
{
    /// <summary>
    /// Provides access to the Schema Registry client, enabling schema management and integration for
    /// message serialization using Avro, Protobuf, or JSON Schema.
    /// </summary>
    ISchemaRegistryClient SchemaRegistryClient { get; }

    /// <summary>
    /// Specifies the initial list of brokers for the Kafka cluster through a comma-separated list of host/port pairs.
    /// This setting is essential for establishing initial connectivity to the Kafka cluster.
    /// </summary>
    /// <param name="bootstrapServers">The bootstrap servers for the Kafka cluster.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator UseBootstrapServers(string bootstrapServers);
    /// <summary>
    /// Conditionally creates a new topic if it does not exist, based on the provided name, number of partitions, and replication factor.
    /// This method allows for preemptive topic setup, ensuring topics are ready for message production and consumption.
    /// </summary>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="numberOfPartitions">The desired number of partitions for the topic.</param>
    /// <param name="replicationFactor">The desired replication factor for the topic.</param>
    /// <param name="timeToRetainDataMs">The period for which data should be retained in the topic, specified as an enum value of <see cref="RetentionPeriodMs"/>.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator CreateTopicIfNotExists(string topicName, int numberOfPartitions, short replicationFactor, RetentionPeriodMs timeToRetainDataMs = RetentionPeriodMs.NoRetention);
    /// <summary>
    /// Configures authentication credentials for secure access to the Kafka cluster, supporting mechanisms that require username and password.
    /// </summary>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator WithCredentials(string username, string password);
    /// <summary>
    /// Sets up the Schema Registry client with detailed configurations, supporting advanced schema management capabilities.
    /// </summary>
    /// <param name="configureAction">An action delegate to configure the Schema Registry, including URL setup, authentication, and compatibility.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator WithSchemaRegistry(Action<IPocKafkaSchemaRegistryConfig> configureAction);
    /// <summary>
    /// Registers a new producer with the ability to send messages to Kafka topics, offering extensive configuration options for tuning producer behavior.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used in Kafka messages, affecting partitioning.</typeparam>
    /// <typeparam name="TValue">The type of the message payload.</typeparam>
    /// <param name="configureAction">An action delegate for configuring the producer, enabling customization of settings such as retry policies and serialization.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator AddProducer<TKey, TValue>(Action<IProducerConfigurationBuilder<TKey, TValue>> configureAction);
    /// <summary>
    /// Integrates a new consumer into the Kafka cluster configuration, enabling message consumption from specified topics with customizable settings.
    /// </summary>
    /// <typeparam name="TKey">The type of the key associated with Kafka messages, used for filtering and identification.</typeparam>
    /// <typeparam name="TValue">The type of the message payload.</typeparam>
    /// <param name="configureAction">An action delegate for detailed consumer setup, facilitating the definition of group IDs, offset behaviors, and more.</param>
    /// <returns>The current <see cref="IClusterConfigurator"/> instance for fluent configuration chaining.</returns>
    IClusterConfigurator AddConsumer<TKey, TValue>(Action<IConsumerConfigurationBuilder<TKey, TValue>> configureAction);
}