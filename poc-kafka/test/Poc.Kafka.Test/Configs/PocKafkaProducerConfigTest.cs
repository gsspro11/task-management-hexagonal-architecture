using Poc.Kafka.Configs;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Configs;

public class PocKafkaProducerConfigTest
{

    [Fact]
    public void NewInstance_WhenValidConfigs_MatchPropertiesSuccessfully()
    {
        string expectedBootstrapServers = "localhost:9092";
        string expectedUsername = "XXXX";
        string expectedPassword = "XXXX";
        string expectedTopic = "test-topic";
        bool expectedEnableIdempotence = false;
        bool expectedApiVersionRequest = true;
        short expectedMessageSendMaxRetries = 3;
        Acks expectedAcks = Acks.Leader;
        short expectedMaxInFlight = 4;
        var expectedTransactionalId = "trans-id";
        int expectedBatchSize = 1000;
        int expectedBatchNumMessages = 100;
        double expectedLingerMs = 10.0;

        var sut = new PocKafkaProducerConfig();
        sut.SetBootstrapServers(expectedBootstrapServers);
        sut.SetTopic(expectedTopic);
        sut.SetApiVersionRequest(expectedApiVersionRequest);
        sut.SetTransactionalId(expectedTransactionalId);
        sut.SetAcks(expectedAcks);
        sut.SetMaxInFlight(expectedMaxInFlight);
        sut.SetMessageSendMaxRetries(expectedMessageSendMaxRetries);
        sut.SetBatchNumMessages(expectedBatchNumMessages);
        sut.SetBatchSize(expectedBatchSize);
        sut.SetLingerMs(expectedLingerMs);

        Assert.Equal(expectedBootstrapServers, sut.BootstrapServers);
        Assert.Equal(expectedTopic, sut.Topic);
        Assert.Equal(expectedEnableIdempotence, sut.EnableIdempotence);
        Assert.Equal(expectedApiVersionRequest, sut.ApiVersionRequest);
        Assert.Equal(expectedMessageSendMaxRetries, sut.MessageSendMaxRetries);
        Assert.Equal(expectedAcks, sut.Acks);
        Assert.Equal(expectedMaxInFlight, sut.MaxInFlight);
        Assert.Contains(expectedTransactionalId, sut.TransactionalId);
        Assert.Equal(expectedBatchSize, sut.BatchSize);
        Assert.Equal(expectedBatchNumMessages, sut.BatchNumMessages);
        Assert.Equal(expectedLingerMs, sut.LingerMs);

        sut.SetCredentials(expectedUsername, expectedPassword);
        Assert.Equal(expectedUsername, sut.Username);
        Assert.Equal(expectedPassword, sut.Password);
        Assert.True(sut.IsCredentialsProvided);

        Assert.False(sut.EnableIdempotence);

        sut.SetIdempotenceEnabled(); // This also sets MaxInFlight, MessageSendMaxRetries, and Acks
        Assert.True(sut.EnableIdempotence);
        Assert.Equal(5, sut.MaxInFlight);
        Assert.Equal(3, sut.MessageSendMaxRetries);
        Assert.Equal(Acks.All, sut.Acks);
    }
}
