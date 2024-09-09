namespace Poc.Kafka.Test.CommonTests.Utilities;

internal static class ArrayUtility
{
    public static byte[] ToBigEndian<T>(T value) where T : struct
    {
        if (!IsSupportedType(typeof(T)))
            throw new InvalidOperationException("Tipo não suportado para conversão BigEndian.");

        var bytes = GetBytesDynamic(value);
        ConvertToBigEndian(bytes);

        return bytes;
    }


    private static bool IsSupportedType(Type type)
    {
        if (!type.IsValueType)
            return false;

        var supportedTypes = new Type[]
        {
            typeof(int), typeof(long), typeof(short),
            typeof(uint), typeof(ulong), typeof(ushort),
            typeof(double), typeof(float)
        };

        return supportedTypes.Contains(type);
    }

    private static byte[] GetBytesDynamic<T>(T value) where T : struct
    {
        var method = typeof(BitConverter).GetMethod("GetBytes", new Type[] { typeof(T) });
        return (byte[])method?.Invoke(null, new object[] { value })!;
    }

    private static void ConvertToBigEndian(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(data);
    }
}