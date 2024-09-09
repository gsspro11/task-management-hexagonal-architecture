using Poc.Kafka.Factories;
using Confluent.Kafka;
using System.Text;

namespace Poc.Kafka.Test.Factories;

public class KafkaMessageFactoryTest
{
    [Fact]
    public void CreateKafkaMessage_When_ReturnsValidMessage()
    {
        string key = "test_key";
        string value = "test_value";
        var headers = new Headers { { "header_key", Encoding.UTF8.GetBytes("header_value") } };

        var message = KafkaMessageFactory.CreateKafkaMessage(value, key, headers);

        Assert.Equal(key, message.Key);
        Assert.Equal(value, message.Value);
        Assert.Equal(headers, message.Headers);
    }
}
