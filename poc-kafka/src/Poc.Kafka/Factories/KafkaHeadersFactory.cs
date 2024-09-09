using Confluent.Kafka;
using System.Text;

namespace Poc.Kafka.Factories;

/// <summary>
/// Provides factory methods for creating Kafka header objects.
/// This utility class includes methods for creating single or multiple Kafka headers
/// for use in producing Kafka messages. It simplifies the creation of headers by providing
/// overloads for common use cases, such as creating headers from string or byte array values.
/// </summary>
public static class KafkaHeadersFactory
{
    /// <summary>
    /// Creates a single Kafka header.
    /// </summary>
    /// <param name="key">The key of the header.</param>
    /// <param name="value">The value of the header as a byte array.</param>
    /// <returns>A Kafka Header instance with the specified key and value.</returns>
    public static Header CreateHeader(string key, byte[] value) =>
      new(key, value);

    /// <summary>
    /// Creates a Kafka Headers collection containing a single header from a byte array value.
    /// </summary>
    /// <param name="key">The key of the header to create.</param>
    /// <param name="value">The byte array value of the header.</param>
    /// <returns>A Kafka Headers collection containing the specified single header.</returns>
    public static Headers CreateSimpleHeader(string key, byte[] value) =>
        new() { CreateHeader(key, value) };

    /// <summary>
    /// Creates a Kafka Headers collection containing a single header from a string value.
    /// This method automatically converts the string value to a byte array using UTF-8 encoding.
    /// </summary>
    /// <param name="key">The key of the header to create.</param>
    /// <param name="value">The string value of the header.</param>
    /// <param name="encoding">The string value of the header.</param>
    /// <returns>A Kafka Headers collection containing the specified single header.</returns>
    public static Headers CreateSimpleHeader(string key, string value, Encoding encoding = default!) =>
        new() { CreateHeader(key, EncodeStringToBytes(value, encoding)) };

    private static byte[] EncodeStringToBytes(string value, Encoding encoding = default!) =>
        (encoding ?? Encoding.UTF8).GetBytes(value);

    /// <summary>
    /// Creates a Kafka Headers collection from a dictionary of key-value pairs.
    /// Each key-value pair in the dictionary is added as a separate header in the collection.
    /// </summary>
    /// <param name="keyValuePairs">A dictionary where each key-value pair represents a header,
    /// with the key as the header key and the value as the header value as a byte array.</param>
    /// <returns>A Kafka Headers collection containing all headers specified in the dictionary.</returns>
    public static Headers CreateMultipleHeaders(Dictionary<string, byte[]> keyValuePairs)
    {
        var headers = new Headers();
        foreach (var pair in keyValuePairs)
        {
            headers.Add(pair.Key, pair.Value);
        }

        return headers;
    }
}