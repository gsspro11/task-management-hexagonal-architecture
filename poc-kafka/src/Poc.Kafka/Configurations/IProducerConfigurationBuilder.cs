using Poc.Kafka.Configs;

namespace Poc.Kafka.Configurations;

/// <summary>
/// Enables fluent configuration of a Kafka producer, tailored with specific key and value types.
/// This interface streamlines the customization of producer settings, including the specification
/// of serialization strategies for keys and values, ensuring that messages are properly formatted
/// when published to Kafka topics.
/// </summary>
/// <typeparam name="TKey">The type of the key in the Kafka messages produced.</typeparam>
/// <typeparam name="TValue">The type of the value in the Kafka messages produced.</typeparam>
public interface IProducerConfigurationBuilder<TKey, TValue>
{
    /// <summary>
    /// Applies additional settings to the producer through a configuration action. This method opens up 
    /// possibilities for fine-tuning the producer's operational parameters and behavior by interacting 
    /// directly with an <see cref="IPocKafkaProducerConfig"/> instance.
    /// </summary>
    /// <param name="configureAction">A delegate that enables the configuration of producer settings.</param>
    /// <returns>The current <see cref="IProducerConfigurationBuilder{TKey, TValue}"/> instance, supporting 
    /// fluent chaining of configuration methods.</returns>
    IProducerConfigurationBuilder<TKey, TValue> Configure(Action<IPocKafkaProducerConfig> configureAction);
    /// <summary>
    /// Specifies custom serialization strategies for the keys and values of Kafka messages. This method 
    /// provides the means to define how data should be converted into a binary format suitable for Kafka 
    /// messaging, accommodating custom or complex data types.
    /// </summary>
    /// <param name="configureAction">A delegate for configuring serialization approaches via an 
    /// <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/>, facilitating the definition of 
    /// custom serializers for both keys and values of messages.</param>
    /// <returns>The current <see cref="IProducerConfigurationBuilder{TKey, TValue}"/> instance, enabling 
    /// seamless continuation of the configuration process.</returns>
    IProducerConfigurationBuilder<TKey, TValue> WithSerialization(Action<ISerializationConfigurationBuilder<TKey, TValue>> configureAction);
}