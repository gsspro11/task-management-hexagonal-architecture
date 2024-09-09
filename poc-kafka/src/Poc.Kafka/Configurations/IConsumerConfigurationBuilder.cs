using Poc.Kafka.Configs;

namespace Poc.Kafka.Configurations;

/// <summary>
/// Enables fluent configuration of a Kafka consumer with specific key and value types, providing a comprehensive 
/// approach to customize consumer settings, including deserialization methods and various consumer behaviors. This 
/// interface allows developers to finely tune consumer operations, group management, and message deserialization to 
/// align with specific application requirements.
/// </summary>
/// <typeparam name="TKey">The type of the key for messages the consumer will process.</typeparam>
/// <typeparam name="TValue">The type of the value for messages the consumer will process.</typeparam>
public interface IConsumerConfigurationBuilder<TKey, TValue>
{
    /// <summary>
    /// Applies additional settings to the consumer configuration. This method offers a direct way to manipulate 
    /// the consumer's detailed settings via an <see cref="IPocKafkaConsumerConfig"/> instance, allowing for the 
    /// customization of key aspects like group ID, offset reset behavior, and fetch parameters. This detailed 
    /// customization enhances the consumer's adaptability to diverse operational requirements and optimization needs.
    /// </summary>
    /// <param name="configureAction">An action delegate for configuring the consumer's settings, providing access to 
    /// modify a wide range of consumer configurations.</param>
    /// <returns>The current <see cref="IConsumerConfigurationBuilder{TKey, TValue}"/> instance to support fluent 
    /// configuration chaining.</returns>
    IConsumerConfigurationBuilder<TKey, TValue> Configure(Action<IPocKafkaConsumerConfig> configureAction);
    /// <summary>
    /// Defines custom serialization and deserialization strategies for the consumer's keys and values, facilitating 
    /// the integration of custom or specialized serialization logic. Through this configuration, developers can 
    /// specify how messages are encoded and decoded, ensuring full compatibility with Kafka's data handling and 
    /// meeting specific application data format requirements.
    /// </summary>
    /// <param name="configureAction">An action delegate that accepts an <see cref="IDeserializationConfigurationBuilder{TKey, TValue}"/> 
    /// instance, enabling the setup of custom serializers and deserializers for the consumer's message keys and values.</param>
    /// <returns>The current <see cref="IConsumerConfigurationBuilder{TKey, TValue}"/> instance to enable continued 
    /// fluent configuration.</returns>
    IConsumerConfigurationBuilder<TKey, TValue> WithSerialization(Action<IDeserializationConfigurationBuilder<TKey, TValue>> configureAction);
}
