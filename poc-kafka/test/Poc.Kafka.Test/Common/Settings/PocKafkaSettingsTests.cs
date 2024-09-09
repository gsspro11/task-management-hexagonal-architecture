using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Settings;

namespace Poc.Kafka.Test.Common.Settings;

public class PocKafkaSettingsTests
{

    [Fact]
    public void PocKafkaSettings_WhenInitialized_PropertiesAreSetCorrectly()
    {
        var expectedConsumerConfig = new PocConsumerSettings()
        {
            Name = "TestConsumer",
            Topics = [
                new() { Topic = "topic1" },
                new() { Topic = "topic2" }
            ],
            GroupId = "group-id",
            SessionTimeoutMs = 30000,
            HeartbeatIntervalMs = 3000,
            DelayPartitionEofMs = ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS,
            RetryDelayMs = 5000,
            RetryLimit = 3,
            TopicRetry = "topic-retry",
            TopicDeadLetter = "topic-dead-letter",
            FetchWaitMaxMs = 500,
            FetchMinBytes = ConsumerConstant.MAX_FETCH_MIN_BYTES,
            FetchMaxBytes = ConsumerConstant.MAX_FETCH_MAX_BYTES,
            MaxPartitionFetchBytes = ConsumerConstant.MAX_PARTITION_FETCH_BYTES,
            ApiVersionRequest = false
        };

        var expectedProducerConfig = new PocProducerSettings()
        {
            Name = "TestProducer",
            Topic = "topic3",
            Acks = Confluent.Kafka.Acks.All,
            ApiVersionRequest = false,
            MessageSendMaxRetries = 3,
            MaxInFlight = 1000000,
            TransactionalId = "transaction",
            TransactionTimeoutMs = 60000,
            BatchSize = 1000000,
            BatchNumMessages = 10000,
            LingerMs = 5
        };

        var expectedSchemaRegistry = new PocSchemaRegistrySettings
        {
            Url = "localhost:8081",
            Username = "username",
            Password = "password"
        };

        var expectedClusterConfig = new PocClusterSettings()
        {
            Name = "TestCluster",
            BootstrapServers = "localhost:9092",
            Username = "testuser",
            Password = "testpassword",
            SchemaRegistrySettings = expectedSchemaRegistry,
            Consumers = [expectedConsumerConfig],
            Producers = [expectedProducerConfig]
        };

        var kafkaSettings = new PocKafkaSettings
        {
            Clusters = [expectedClusterConfig]
        };

        // Assert
        Assert.NotNull(kafkaSettings);
        Assert.NotNull(kafkaSettings.Clusters);
        Assert.Single(kafkaSettings.Clusters);

        var clusterConfig = kafkaSettings.Clusters[0];
        Assert.Equal(expectedClusterConfig, clusterConfig);
        Assert.Equal(expectedSchemaRegistry, clusterConfig.SchemaRegistrySettings);

        Assert.NotNull(clusterConfig.Consumers);
        Assert.Single(clusterConfig.Consumers);
        Assert.Equal(expectedConsumerConfig, clusterConfig.Consumers[0]);

        Assert.NotNull(clusterConfig.Producers);
        Assert.Single(clusterConfig.Producers);
        Assert.Equal(expectedProducerConfig, clusterConfig.Producers[0]);
    }
}
