using Confluent.Kafka;

namespace Poc.Kafka.Factories;

/// <summary>
/// A factory for creating Kafka messages.
/// </summary>
public static class KafkaMessageFactory
{
    /// <summary>
    /// Creates a Kafka message with both key and value being optionally null.
    /// This method supports creating messages where either, both, or none of the
    /// key and value are provided, making it adaptable for various messaging needs.
    /// Headers can also be optionally included to attach metadata to the message.
    /// </summary>
    /// <typeparam name="TKey">The type of the key, which can be null.</typeparam>
    /// <typeparam name="TValue">The type of the value, which can be null.</typeparam>
    /// <param name="value">The value of the message, which can be null.</param>
    /// <param name="key">The key of the message, which can be null.</param>
    /// <param name="headers">Optional headers to include with the message.</param>
    /// <returns>A Kafka message configured with the provided key, value, and optional headers.</returns>
    public static Message<TKey, TValue> CreateKafkaMessage<TKey, TValue>(
        TValue? value = default,
        TKey? key = default,
        Headers? headers = null) => new()
        {
            Key = key!,
            Value = value!,
            Headers = headers
        };
}