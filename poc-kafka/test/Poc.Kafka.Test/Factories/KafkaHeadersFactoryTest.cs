using Poc.Kafka.Factories;

namespace Poc.Kafka.Test.Factories;

public class KafkaHeadersFactoryTest
{

    [Fact]
    public void CreateMultipleHeaders_WhenHeadersKeyNonExistent_ReturnsDefaultValue()
    {
        var headers = new Dictionary<string, byte[]>();

        var actualHeaders = KafkaHeadersFactory.CreateMultipleHeaders(headers);

        Assert.Empty(actualHeaders);
    }

    [Fact]
    public void CreateMultipleHeaders_WhenHeadersExist_ReturnsHeadersWithSameValues()
    {
        var headersDict = new Dictionary<string, byte[]>
        {
            { "header1", new byte[] { 1, 2, 3 } },
            { "header2", new byte[] { 4, 5, 6 } },
            { "header3", new byte[] { 7, 8, 9 } }
        };

        var headers = KafkaHeadersFactory.CreateMultipleHeaders(headersDict);

        Assert.Equal(headersDict.Count, headers.Count);

        foreach (var header in headersDict)
        {
            Assert.True(headers.TryGetLastBytes(header.Key, out var actualValue));
            Assert.Equal(header.Value, actualValue);
        }
    }
}
