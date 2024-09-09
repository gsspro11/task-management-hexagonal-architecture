using Poc.Kafka.Common.Settings;
using Poc.Kafka.Configs;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Configs;

public class PocKafkaConsumerConfigTest
{
    [Fact]
    public void NewInstance_WithValidConfigs_MatchPropertiesSuccessfully()
    {
        string expectedBootstrapServers = "localhost:9092";
        bool expectedApiVersionRequest = true;
        string expectedUsername = "XXXX";
        string expectedPassword = "XXXX";
        var expectedTopics = new List<PocTopicSettings>
        {
            new() { Topic = "test-topic-1" },
            new() { Topic = "test-topic-2" }
        };
        string expectedGroupId = "test-group-id";
        int expectedSessionTimeoutMs = 15000;
        int expectedHeartbeatIntervalMs = 3000;
        int expectedStatisticsIntervalMs = 1000;
        int expectedMaxPollIntervalMs = 300000;
        int expectedRetryLimit = 5;
        string expectedRetryTopic = "retry-topic";
        string expectedTopicDeadLetter = "dead-letter-topic";
        int expectedRetryDelayMs = 100;
        AutoOffsetReset expectedAutoOffsetReset = AutoOffsetReset.Earliest;
        int expectedDelayPartitionEofMs = 2000;

        int expectedFetchWaitMaxMs = 500;
        int expectedFetchMinBytes = 10;
        int expectedFetchMaxBytes = 52428800;
        int expectedMaxPartitionFetchBytes = 1048576;

        var sut = new PocKafkaConsumerConfig();
        sut.SetBootstrapServers(expectedBootstrapServers);
        sut.SetApiVersionRequest(expectedApiVersionRequest);
        sut.SetCredentials(expectedUsername, expectedPassword);
        sut.SetTopics(expectedTopics);
        sut.SetGroupId(expectedGroupId);
        sut.SetSessionTimeoutMs(expectedSessionTimeoutMs);
        sut.SetHeartbeatIntervalMs(expectedHeartbeatIntervalMs);
        sut.SetStatisticsIntervalMs(expectedStatisticsIntervalMs);
        sut.SetMaxPollIntervalMs(expectedMaxPollIntervalMs);
        sut.SetEnableAutoCommit();
        sut.SetEnableRetryTopicConsumer();
        sut.SetRetryLimit(expectedRetryLimit);
        sut.SetTopicRetry(expectedRetryTopic);
        sut.SetTopicDeadLetter(expectedTopicDeadLetter);
        sut.SetRetryDelayMs(expectedRetryDelayMs);
        sut.SetAutoOffsetReset(expectedAutoOffsetReset);
        sut.SetDelayPartitionEofMs(expectedDelayPartitionEofMs);
        sut.SetFetchWaitMaxMs(expectedFetchWaitMaxMs);
        sut.SetFetchMinBytes(expectedFetchMinBytes);
        sut.SetFetchMaxBytes(expectedFetchMaxBytes);
        sut.SetMaxPartitionFetchBytes(expectedMaxPartitionFetchBytes);

        Assert.Equal(expectedBootstrapServers, sut.BootstrapServers);
        Assert.True(sut.ApiVersionRequest);
        Assert.Equal(expectedUsername, sut.Username);
        Assert.Equal(expectedPassword, sut.Password);
        Assert.True(sut.IsCredentialsProvided);
        Assert.Equivalent(expectedTopics, sut.Topics);
        Assert.Equal(expectedGroupId, sut.GroupId);
        Assert.Equal(expectedSessionTimeoutMs, sut.SessionTimeoutMs);
        Assert.Equal(expectedHeartbeatIntervalMs, sut.HeartbeatIntervalMs);
        Assert.Equal(expectedStatisticsIntervalMs, sut.StatisticsIntervalMs);
        Assert.Equal(expectedMaxPollIntervalMs, sut.MaxPollIntervalMs);
        Assert.True(sut.EnableAutoCommit);
        Assert.True(sut.EnableRetryTopicConsumer);
        Assert.Equal(expectedRetryLimit, sut.RetryLimit);
        Assert.Equal(expectedRetryTopic, sut.TopicRetry);
        Assert.Equal(expectedTopicDeadLetter, sut.TopicDeadLetter);
        Assert.Equal(expectedRetryDelayMs, sut.RetryDelayMs);
        Assert.Equal(expectedAutoOffsetReset, sut.AutoOffsetReset);
        Assert.Equal(expectedDelayPartitionEofMs, sut.DelayPartitionEofMs);
        Assert.Equal(expectedFetchWaitMaxMs, sut.FetchWaitMaxMs);
        Assert.Equal(expectedFetchMinBytes, sut.FetchMinBytes);
        Assert.Equal(expectedFetchMaxBytes, sut.FetchMaxBytes);
        Assert.Equal(expectedMaxPartitionFetchBytes, sut.MaxPartitionFetchBytes);
    }
}
