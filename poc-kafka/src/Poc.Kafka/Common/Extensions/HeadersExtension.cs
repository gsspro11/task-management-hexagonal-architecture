using Confluent.Kafka;
using System.Globalization;
using System.Text;

namespace Poc.Kafka.Common.Extensions;

/// <summary>
/// Provides extension methods for Kafka Headers to facilitate getting and setting header values more conveniently.
/// </summary>
public static class HeadersExtension
{
    private static readonly Dictionary<Type, Func<byte[], object>> Converters = new()
    {
        { typeof(int), bytes => BitConverter.ToInt32(bytes, 0) },
        { typeof(long), bytes => BitConverter.ToInt64(bytes, 0) },
        { typeof(bool), bytes => BitConverter.ToBoolean(bytes, 0) },
        { typeof(double), bytes => BitConverter.ToDouble(bytes, 0) }
        // Additional converters can be registered using RegisterConverter method.
    };

    /// <summary>
    /// Registers a custom converter for converting header bytes to a specific type.
    /// </summary>
    /// <typeparam name="T">The target type for the conversion.</typeparam>
    /// <param name="converter">A function that converts an array of bytes to the specified type.</param>
    /// <remarks>
    /// This method allows for the extension of the conversion capabilities to support custom types beyond the built-in ones.
    /// </remarks>
    public static void RegisterConverter<T>(Func<byte[], T> converter) =>
        Converters[typeof(T)] = bytes => converter(bytes)!;

    /// <summary>
    /// Retrieves a header value as a specified type from Kafka message headers.
    /// </summary>
    /// <typeparam name="T">The type to which the header value should be converted.</typeparam>
    /// <param name="headers">The collection of headers from a Kafka message.</param>
    /// <param name="headerKey">The key of the header to retrieve.</param>
    /// <param name="encoding">The encoding to use for converting header bytes to a string if needed. Defaults to UTF8 if not specified.</param>
    /// <returns>The header value converted to the specified type, or the default value of the type if the conversion fails or the header does not exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the headerKey is null.</exception>
    /// <exception cref="InvalidCastException">Thrown if the conversion of the header value to the specified type fails.</exception>
    public static T? GetHeaderAs<T>(this Headers headers, string headerKey, Encoding encoding = default!)
    {

        ArgumentException.ThrowIfNullOrWhiteSpace(headerKey);

        encoding ??= Encoding.UTF8;

        if (headers != null && headers.TryGetLastBytes(headerKey, out byte[] headerBytes))
        {
            try
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)encoding.GetString(headerBytes);

                if (Converters.TryGetValue(typeof(T), out var converter))
                    return (T)converter(headerBytes);

                string headerString = encoding.GetString(headerBytes);
                return (T)Convert.ChangeType(headerString, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert header '{headerKey}' to type {typeof(T).Name}.", ex);
            }
        }

        return default;
    }

    /// <summary>
    /// Adds or updates a header in the Kafka message headers.
    /// </summary>
    /// <param name="headers">The collection of headers from a Kafka message.</param>
    /// <param name="headerKey">The key of the header to add or update.</param>
    /// <param name="headerValue">The value of the header as a byte array.</param>
    /// <remarks>
    /// If the header already exists, it is removed before the new value is added, ensuring that the header value is updated.
    /// This method offers a convenient way to manage header values within Kafka messages.
    /// </remarks>
    public static void AddOrUpdate(this Headers headers, string headerKey, byte[] headerValue)
    {
        ArgumentNullException.ThrowIfNull(headers);

        if (headers.TryGetLastBytes(headerKey, out _))
            headers.Remove(headerKey);

        headers.Add(headerKey, headerValue);
    }
}
