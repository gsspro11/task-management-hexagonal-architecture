using Poc.Kafka.Common;
using Poc.Kafka.Common.Serdes;
using Poc.Kafka.Test.CommonTests.Fakes;
using Poc.Kafka.Test.CommonTests.Utilities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;

namespace Poc.Kafka.Test.Common.Serdes.Deserializers;

public class JsonDeserializerTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger = new();


    [Fact]
    public void Deserialize_WhenNull_ReturnsDefaultValue()
    {
        var sut = new JsonDeserializer<string>(_mockLogger.Object);

        string result = sut.Deserialize(null!, true, new SerializationContext());

        Assert.Null(result);
    }


    [Fact]
    public void Deserialize_WhenString_ReturnsCorrectString()
    {
        var sut = new JsonDeserializer<string>(_mockLogger.Object);

        string expectedvalue = "Test string";
        byte[] data = Encoding.UTF8.GetBytes(expectedvalue);

        string result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedvalue, result);
    }

    [Fact]
    public void Deserialize_WhenInt_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<int>(_mockLogger.Object);
        int expectedValue = 123;

        byte[] data = ArrayUtility.ToBigEndian(expectedValue);

        int result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedValue, result);
    }


    [Fact]
    public void Deserialize_WhenLong_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<long>(_mockLogger.Object);
        long expectedValue = 123L;

        byte[] data = ArrayUtility.ToBigEndian(expectedValue);

        long result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Deserialize_WhenUInt_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<uint>(_mockLogger.Object);
        uint expectedValue = 123U;

        byte[] data = ArrayUtility.ToBigEndian(expectedValue);

        uint result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Deserialize_WhenULong_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<ulong>(_mockLogger.Object);
        ulong expectedValue = 123UL;

        byte[] data = ArrayUtility.ToBigEndian(expectedValue);

        ulong result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedValue, result);
    }


    [Fact]
    public void Deserialize_WhenGuid_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<Guid>(_mockLogger.Object);

        Guid expectedvalue = Guid.NewGuid();
        byte[] data = expectedvalue.ToByteArray();

        Guid result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Equal(expectedvalue, result);
    }


    [Fact]
    public void Deserialize_WhenByteArray_ReturnsCorrectValue()
    {
        var sut = new JsonDeserializer<byte[]>(_mockLogger.Object);
        byte[] expectedValue = new byte[] { 1, 2, 3, 4 };

        byte[] result = sut.Deserialize(expectedValue, false, new SerializationContext());

        Assert.Equal(expectedValue, result);
    }


    [Fact]
    public void Deserialize_WhenComplexType_ReturnsCorrectObject()
    {
        var deserializer = new JsonDeserializer<JsonRecordFake>(_mockLogger.Object);
        var messageFake = new JsonRecordFake(Message: "Test Message");
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(messageFake);

        var result = deserializer.Deserialize(data, false, new SerializationContext());

        Assert.NotNull(result);
        Assert.Equal(messageFake.Message, result.Message);
    }

    [Fact]
    public void Deserialize_WhenInvalidJson_ThrowsJsonException()
    {
        var sut = new JsonDeserializer<JsonRecordFake>(_mockLogger.Object);
        byte[] data = Encoding.UTF8.GetBytes("Invalid JSON");

        var result = sut.Deserialize(data, false, new SerializationContext());

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_WhenSerializerNotProvidedForType_ThrowsInvalidOperationException()
    {
        var sut = new JsonDeserializer<double>(_mockLogger.Object);
        byte[] data = ArrayUtility.ToBigEndian(10.0d);

        string expectedErrorMessage = "Unsupported primitive type System.Double for deeserialization.";

        Exception ex = Assert.Throws<InvalidOperationException>(() => sut.Deserialize(data, false, new SerializationContext()));
        Assert.Equal(expectedErrorMessage, ex.Message);
    }
}


