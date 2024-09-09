using Confluent.Kafka;
using System.Text.Json;
namespace Poc.Kafka.Common.Serdes;

public sealed class JsonSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        if (data is null)
            return [];

        if (data is string strData)
            return SerializeString(strData, context);

        if (data is Guid guidData) // UUIDBinary
            return SerializeGuid(guidData);

        if (data is byte[] byteArray) // Base64
            return byteArray;

        if (data.GetType().IsPrimitive)
            return SerializePrimitive(data, context);

        return JsonSerializer.SerializeToUtf8Bytes(data);
    }

    private static byte[] SerializeString(string data, SerializationContext context) =>
        Serializers.Utf8.Serialize(data, context);

    private static byte[] SerializeGuid(Guid data)
    {
        if (data == Guid.Empty)
            return [];

        return data.ToByteArray();
    }

    private static byte[] SerializePrimitive(T data, SerializationContext context)
    {
        return data switch
        {
            int intValue => Serializers.Int32.Serialize(intValue, context),
            long longValue => Serializers.Int64.Serialize(longValue, context),
            uint uintValue => CustomSerializers.UInt32.Serialize(uintValue, context),
            ulong ulongValue => CustomSerializers.UInt64.Serialize(ulongValue, context),
            _ => throw new InvalidOperationException($"Unsupported primitive type {typeof(T)} for serialization."),
        };
    }

    private static class CustomSerializers
    {
        internal static readonly ISerializer<ulong> UInt64 = new UInt64Serializer();
        internal static readonly ISerializer<uint> UInt32 = new UInt32Serializer();

        private sealed class UInt32Serializer : ISerializer<uint>
        {
            public byte[] Serialize(uint data, SerializationContext context)
            {
                return
                [
                    (byte)(data >> 24),
                    (byte)(data >> 16),
                    (byte)(data >> 8),
                    (byte)data
                ];
            }
        }
        private sealed class UInt64Serializer : ISerializer<ulong>
        {
            public byte[] Serialize(ulong data, SerializationContext context)
            {
                return
                [
                    (byte)(data >> 56),
                    (byte)(data >> 48),
                    (byte)(data >> 40),
                    (byte)(data >> 32),
                    (byte)(data >> 24),
                    (byte)(data >> 16),
                    (byte)(data >> 8),
                    (byte)data
                ];
            }
        }

    }
}
