using Poc.Kafka.Common.Serdes;
using Poc.Kafka.Test.CommonTests.Fakes;
using Poc.Kafka.Test.CommonTests.Utilities;
using Confluent.Kafka;
using System.Text;


namespace Poc.Kafka.Test.Common.Serdes.Serializers;

public class JsonSerializerTest
{

    [Fact]
    public void Serialize_WhenGuid_ReturnsArrayEmpty()
    {
        var sut = new JsonSerializer<Guid>();

        var data = Guid.NewGuid();

        var expectedValue = data.ToByteArray();

        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        Assert.Equal(expectedValue, serializedData);
    }


    [Fact]
    public void Serialize_WheByteArray_ReturnsArrayEmpty()
    {
        var sut = new JsonSerializer<byte[]>();

        var expectedValue = new byte[] { 1, 2, 3, 4 };

        byte[] serializedData = sut.Serialize(expectedValue, new SerializationContext());

        Assert.Equal(expectedValue, serializedData);
    }


    [Fact]
    public void Serialize_WhenNull_ReturnsArrayEmpty()
    {
        var sut = new JsonSerializer<string>();

        byte[] serializedData = sut.Serialize(null!, new SerializationContext());

        Assert.Empty(serializedData);
    }

    [Fact]
    public void Serialize_WhenString_ReturnsUtf8EncodedByteArray()
    {
        var sut = new JsonSerializer<string>();
        var data = "Hello World";
        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        byte[] expectedValue = Encoding.UTF8.GetBytes(data);
        Assert.Equal(expectedValue, serializedData);
    }

    [Theory]
    [InlineData(123)]
    [InlineData(-456)]
    public static void Serialize_GivenIntValues_ReturnsCorrectByteArray(int data)
    {
        var sut = new JsonSerializer<int>();
        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        byte[] expectedData = ArrayUtility.ToBigEndian(data);

        Assert.Equal(expectedData, serializedData);
    }

    [Theory]
    [InlineData(123L)]
    [InlineData(-456L)]
    [InlineData(0L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public static void Serialize_GivenLongValues_ReturnsCorrectByteArray(long data)
    {
        var sut = new JsonSerializer<long>();
        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        byte[] expectedData = ArrayUtility.ToBigEndian(data);

        Assert.Equal(expectedData, serializedData);
    }

    [Fact]
    public void Serialize_WhenComplexType_ReturnsJsonByteArray()
    {
        var sut = new JsonSerializer<JsonRecordFake>();
        var sampleObject = new JsonRecordFake { Message = "Test" };
        byte[] serializedData = sut.Serialize(sampleObject, new SerializationContext());

        byte[] expectedData = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(sampleObject);
        Assert.Equal(expectedData, serializedData);
    }

    [Fact]
    public void Serialize_WhenUInt_ReturnsCorrectByteArray()
    {
        var sut = new JsonSerializer<uint>();
        uint data = 123U;

        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        byte[] expectedData = ArrayUtility.ToBigEndian(data);

        Assert.Equal(expectedData, serializedData);
    }

    [Fact]
    public void Serialize_WhenULong_ReturnsCorrectByteArray()
    {
        var sut = new JsonSerializer<ulong>();
        ulong data = 123UL;

        byte[] serializedData = sut.Serialize(data, new SerializationContext());

        byte[] expectedData = ArrayUtility.ToBigEndian(data);

        Assert.Equal(expectedData, serializedData);
    }


    [Fact]
    public void Serialize_WhenUnsupportedPrimitiveType_ThrowsInvalidOperationException()
    {
        var sut = new JsonSerializer<float>();
        float data = 123.45f;

        Assert.Throws<InvalidOperationException>(() => sut.Serialize(data, new SerializationContext()));
    }

    [Fact]
    public void Deserialize_WhenSerializerNotProvidedForType_ThrowsInvalidOperationException()
    {
        var sut = new JsonSerializer<double>();
        double data = 10.0d;

        string expectedErrorMessage = "Unsupported primitive type System.Double for serialization.";

        Exception ex = Assert.Throws<InvalidOperationException>(() => sut.Serialize(data, new SerializationContext()));
        Assert.Equal(expectedErrorMessage, ex.Message);
    }
}
