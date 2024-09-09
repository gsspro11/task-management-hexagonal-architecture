using Confluent.Kafka;

namespace Poc.Kafka.Configurations;

/// <summary>
/// Provides a fluent interface for configuring custom deserialization strategies for Kafka consumer message keys and values.
/// Extends <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/> to incorporate deserialization, 
/// facilitating comprehensive control over the serialization and deserialization processes of Kafka message components.
/// </summary>
/// <typeparam name="TKey">The type of the message key to be deserialized.</typeparam>
/// <typeparam name="TValue">The type of the message value to be deserialized.</typeparam>
public interface IDeserializationConfigurationBuilder<TKey, TValue> : ISerializationConfigurationBuilder<TKey, TValue>
{
    /// <summary>
    /// Configures a custom deserializer for message keys, enabling the conversion from a binary format to the specified type <typeparamref name="TKey"/>.
    /// This is crucial for ensuring that keys are correctly interpreted by the consuming application according to its domain-specific data types.
    /// </summary>
    /// <param name="deserializer">The custom deserializer to apply to message keys, encapsulating the logic for transforming byte arrays into <typeparamref name="TKey"/>.</param>
    /// <returns>The current <see cref="IDeserializationConfigurationBuilder{TKey, TValue}"/> instance, supporting fluent configuration chaining.</returns>
    IDeserializationConfigurationBuilder<TKey, TValue> SetKeyDeserializer(IDeserializer<TKey> deserializer);
    /// <summary>
    /// Configures a custom deserializer for message values, enabling the conversion from a binary format to the specified type <typeparamref name="TValue"/>.
    /// This allows for flexible and precise control over how message contents are processed and interpreted when consumed from Kafka topics.
    /// </summary>
    /// <param name="deserializer">The custom deserializer to apply to message values, detailing how byte arrays are transformed into <typeparamref name="TValue"/>.</param>
    /// <returns>The current <see cref="IDeserializationConfigurationBuilder{TKey, TValue}"/> instance, enabling further customization through fluent chaining.</returns>
    IDeserializationConfigurationBuilder<TKey, TValue> SetValueDeserializer(IDeserializer<TValue> deserializer);
}
