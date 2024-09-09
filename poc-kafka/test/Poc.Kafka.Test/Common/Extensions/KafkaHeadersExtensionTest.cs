using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Factories;
using Confluent.Kafka;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Poc.Kafka.Test.Common.Extensions;

public class KafkaHeadersExtensionTest
{
    [Fact]
    public void GetHeaderAs_WhenHeadersIsNull_ReturnsDefaultValue()
    {
        Headers sut = null!;

        int headerValue = sut.GetHeaderAs<int>("key");

        Assert.Equal(0, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenHeadersKeyNonExistent_ReturnsDefaultValue()
    {
        var headers = KafkaHeadersFactory.CreateSimpleHeader("nonexistent-header-key", "");

        int actualHeaderValue = headers.GetHeaderAs<int>("header-key-to-search");

        Assert.Equal(0, actualHeaderValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueIsString_ReturnsExpectedValue()
    {
        string headerKey = "header_as_string";
        string expecteHeaderdValue = "3";

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, expecteHeaderdValue);

        string headerValue = sut.GetHeaderAs<string>(headerKey)!;

        Assert.Equal(expecteHeaderdValue, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueIsInt32_ReturnsExpectedValue()
    {
        string headerKey = "header_as_int32";
        int expectedHeaderdValue = 3;

        byte[] headerValueAsByte = BitConverter.GetBytes(expectedHeaderdValue);

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        int headerValue = sut.GetHeaderAs<int>(headerKey)!;

        Assert.Equal(expectedHeaderdValue, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueIsInt64_ReturnsExpectedValue()
    {
        string headerKey = "header_as_int64";
        long expectedHeaderdValue = long.MaxValue;

        byte[] headerValueAsByte = BitConverter.GetBytes(expectedHeaderdValue);

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        long headerValue = sut.GetHeaderAs<long>(headerKey)!;

        Assert.Equal(expectedHeaderdValue, headerValue);
    }


    [Fact]
    public void GetHeaderAs_WhenValueIsBoolean_ReturnsExpectedValue()
    {
        string headerKey = "header_as_boolean";
        bool expectedHeaderdValue = false;

        byte[] headerValueAsByte = BitConverter.GetBytes(expectedHeaderdValue);

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        bool headerValue = sut.GetHeaderAs<bool>(headerKey)!;

        Assert.Equal(expectedHeaderdValue, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueIsDouble_ReturnsExpectedValue()
    {
        string headerKey = "header_as_double";
        double expectedHeaderdValue = double.MaxValue;

        byte[] headerValueAsByte = BitConverter.GetBytes(expectedHeaderdValue);

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        double headerValue = sut.GetHeaderAs<double>(headerKey)!;

        Assert.Equal(expectedHeaderdValue, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueIsDecimal_ReturnsExpectedValue()
    {
        string headerKey = "header_as_decimal";
        decimal expectedHeaderdValue = 1000.0m;

        byte[] headerValueAsByte = Encoding.UTF8.GetBytes(expectedHeaderdValue.ToString(CultureInfo.InvariantCulture));

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        decimal headerValue = sut.GetHeaderAs<decimal>(headerKey)!;

        Assert.Equal(expectedHeaderdValue, headerValue);
    }

    [Fact]
    public void GetHeaderAs_WhenValueBigInteger_ThrowsInvalidCastException()
    {
        string headerKey = "header_as_big_integer";
        byte[] headerValueAsByte = new BigInteger(1).ToByteArray();

        string expectedErrorMessage = "Failed to convert header 'header_as_big_integer' to type BigInteger.";

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        Exception ex = Assert.Throws<InvalidCastException>(() => sut.GetHeaderAs<BigInteger>(headerKey));
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void GetHeaderAs_WhenRegisterConverter_ReturnsExpectedValue()
    {
        string headerKey = "header_as_guid";
        Guid expectedHeaderdValue = Guid.NewGuid();

        byte[] headerValueAsByte = expectedHeaderdValue.ToByteArray();

        static Guid guidConverter(byte[] bytes) => new(bytes);

        HeadersExtension.RegisterConverter(guidConverter);

        var sut = KafkaHeadersFactory.CreateSimpleHeader(headerKey, headerValueAsByte);

        Guid headerValue = sut.GetHeaderAs<Guid>(headerKey);
        Assert.Equal(expectedHeaderdValue, headerValue);
    }
}
