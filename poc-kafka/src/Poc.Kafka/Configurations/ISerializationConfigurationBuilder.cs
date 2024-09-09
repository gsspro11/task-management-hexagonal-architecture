using Confluent.Kafka;

namespace Poc.Kafka.Configurations;

/// <summary>
/// Facilitates the fluent configuration of serialization strategies for Kafka message keys and values,
/// providing options for both synchronous and asynchronous serialization methods. This interface ensures
/// that message data is correctly serialized into a byte array format, suitable for Kafka messaging systems,
/// and allows for the integration of custom serialization logic, including the potential for non-blocking
/// serialization operations.
/// </summary>
/// <typeparam name="TKey">The data type of the message key to be serialized.</typeparam>
/// <typeparam name="TValue">The data type of the message value to be serialized.</typeparam>
public interface ISerializationConfigurationBuilder<TKey, TValue>
{
    /// <summary>
    /// Configures a custom serializer for message keys, enabling the transformation of key data from its 
    /// native type into a binary format understood by Kafka.
    /// </summary>
    /// <param name="serializer">The serializer instance responsible for the key serialization process.</param>
    /// <returns>The current <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/> instance, allowing
    /// for additional configuration chaining.</returns>
    ISerializationConfigurationBuilder<TKey, TValue> SetKeySerializer(ISerializer<TKey> serializer);
    /// <summary>
    /// Configures a custom serializer for message values, facilitating the conversion of value data from its
    /// original type to a byte array for Kafka messaging compatibility.
    /// </summary>
    /// <param name="serializer">The serializer instance tasked with value serialization duties.</param>
    /// <returns>The current <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/> instance, permitting
    /// further fluent configuration modifications.</returns>
    ISerializationConfigurationBuilder<TKey, TValue> SetValueSerializer(ISerializer<TValue> serializer);
    /// <summary>
    /// Defines a custom asynchronous serializer for message keys, offering an advanced serialization approach
    /// that may leverage non-blocking IO for enhanced efficiency, especially suitable for large data sets or
    /// operations requiring IO-bound tasks.
    /// </summary>
    /// <param name="serializer">The asynchronous serializer for the message key, allowing for async operations
    /// in the serialization process.</param>
    /// <returns>The current <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/> instance, supporting
    /// seamless continuation of configuration setup.</returns>
    ISerializationConfigurationBuilder<TKey, TValue> SetKeyAsyncSerializer(IAsyncSerializer<TKey> serializer);
    /// <summary>
    /// Establishes a custom asynchronous serializer for message values, integrating asynchronous serialization
    /// capabilities to potentially enhance performance through non-blocking operations, ideal for handling
    /// complex serialization logic or large data sizes.
    /// </summary>
    /// <param name="serializer">The asynchronous serializer for the message value, enabling async serialization
    /// tasks.</param>
    /// <returns>The current <see cref="ISerializationConfigurationBuilder{TKey, TValue}"/> instance, encouraging
    /// continued fluent configuration adjustments.</returns>
    ISerializationConfigurationBuilder<TKey, TValue> SetValueAsyncSerializer(IAsyncSerializer<TValue> serializer);
}