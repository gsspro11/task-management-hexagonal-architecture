using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace Poc.Kafka.Common.Serdes;

public sealed class JsonDeserializer<T> : IDeserializer<T>
{
    private readonly Encoding _encoder;
    private readonly ILogger _logger;

    public JsonDeserializer(ILogger<IPocKafkaPubSub> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _encoder = Encoding.UTF8;
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default!;

        try
        {
            Type type = typeof(T);

            if (type == typeof(string))
                return DeserializeString(data, context);

            if (type.IsPrimitive)
                return DeserializePrimitive(data, type, context);

            if (type == typeof(Guid) || type == typeof(Guid?)) // UUIDBinary
                return DeserializeGuid(data, type);

            if (type == typeof(byte[])) // Base64
                return CastToGeneric(data.ToArray());

            return JsonSerializer.Deserialize<T>(_encoder.GetString(data))!;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing message: {Message}", _encoder.GetString(data));
            return default!;
        }
    }
    private static T DeserializeString(ReadOnlySpan<byte> data, SerializationContext context) =>
        CastToGeneric(Deserializers.Utf8.Deserialize(data, false, context));

    private static T DeserializePrimitive(ReadOnlySpan<byte> data, Type type, SerializationContext context)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Int32 => CastToGeneric(Deserializers.Int32.Deserialize(data, false, context)),
            TypeCode.Int64 => CastToGeneric(Deserializers.Int64.Deserialize(data, false, context)),
            TypeCode.UInt32 => CastToGeneric(CustomDeserializers.UInt32.Deserialize(data, false, context)),
            TypeCode.UInt64 => CastToGeneric(CustomDeserializers.UInt64.Deserialize(data, false, context)),
            _ => throw new InvalidOperationException($"Unsupported primitive type {type} for deeserialization."),
        };
    }

    private static T DeserializeGuid(ReadOnlySpan<byte> data, Type type)
    {
        object result;
        if (data.IsEmpty || data.Length != 16)
            result = type == typeof(Guid?) ? (object)(Guid?)null! : Guid.Empty;
        else
            result = new Guid(data.ToArray());

        return CastToGeneric(result);
    }

    private static T CastToGeneric(object value) => (T)value;


    private static class CustomDeserializers
    {
        internal readonly static IDeserializer<uint> UInt32 = new UInt32Deserializer();
        internal readonly static IDeserializer<ulong> UInt64 = new UInt64Deserializer();

        private sealed class UInt32Deserializer : IDeserializer<uint>
        {
            public uint Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull)
                    throw new ArgumentNullException(nameof(isNull), "Null data encountered deserializing UInt32 value");

                if (data.Length != 4)
                {
                    DefaultInterpolatedStringHandler handler = new(79, 1);
                    handler.AppendLiteral("Deserializer<UInt32> encountered data of length ");
                    handler.AppendFormatted(data.Length);
                    handler.AppendLiteral(". Expecting data length to be 4.");
                    throw new ArgumentException(handler.ToStringAndClear());
                }

                return (uint)(data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3]);
            }
        }

        private sealed class UInt64Deserializer : IDeserializer<ulong>
        {
            public ulong Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
            {
                if (isNull)
                    throw new ArgumentNullException(nameof(isNull), "Null data encountered deserializing UInt64 value.");

                if (data.Length != 8)
                {
                    DefaultInterpolatedStringHandler handler = new(78, 1);
                    handler.AppendLiteral("Deserializer<UInt64> encountered data of length ");
                    handler.AppendFormatted(data.Length);
                    handler.AppendLiteral(". Expecting data length to be 8.");
                    throw new ArgumentException(handler.ToStringAndClear());
                }

                return (ulong)data[0] << 56 | (ulong)data[1] << 48 | (ulong)data[2] << 40 |
                       (ulong)data[3] << 32 | (ulong)data[4] << 24 | (ulong)data[5] << 16 |
                       (ulong)data[6] << 8 | data[7];
            }
        }
    }
}