using Poc.Kafka.Common.Constants;
using Poc.Kafka.Configurations;
using Confluent.Kafka;

namespace Poc.Kafka.Test.CommonTests.Utilities;

internal static class ConsumerConfigurationUtility
{
    public static ConsumerConfiguration<TKey, TValue> CreateConsumerConfiguration<TKey, TValue>(
        Action<ConsumerConfigurationBuilder<TKey, TValue>> configure)
    {
        var consumerConfigurationBuilder = CreateConsumerConfigurationBuilder<TKey, TValue>();
        configure(consumerConfigurationBuilder);
        var consumerConfiguration = consumerConfigurationBuilder.Build();
        return consumerConfiguration;
    }

    public static ConsumerConfigurationBuilder<TKey, TValue> CreateConsumerConfigurationBuilder<TKey, TValue>()
    {
        var consumerConfigurationBuilder = new ConsumerConfigurationBuilder<TKey, TValue>(
            bootstrapServers: "localhost:9092",
            username: "username",
            password: "password");

        consumerConfigurationBuilder.Configure(configure =>
        {
            configure.SetName("consumer-unique-name");
            configure.SetTopics([new() { Topic = TestConstant.TOPIC_FAKE }]);
            configure.SetGroupId("group-id");
            configure.SetDelayPartitionEofMs(1000);
            configure.SetAutoOffsetReset(AutoOffsetReset.Earliest);
        });

        return consumerConfigurationBuilder;
    }

    public static void ConfigureRetryAndDeadLetter<TKey, TValue>(
        ConsumerConfigurationBuilder<TKey, TValue> consumerConfigurationBuilder, string? topicRetry = null)
    {
        consumerConfigurationBuilder.Configure(configure =>
        {
            configure.SetEnableRetryTopicConsumer();
            configure.SetRetryLimit(3);
            configure.SetTopicRetry(topicRetry ?? TestConstant.TOPIC_RETRY_FAKE);
            configure.SetRetryDelayMs(ConsumerConstant.MAX_RETRY_DELAY_MS);
            configure.SetTopicDeadLetter(TestConstant.TOPIC_DLQ_FAKE);
        });
    }

    public static void ConfigureDeadLetter<TKey, TValue>(
        ConsumerConfigurationBuilder<TKey, TValue> consumerConfigurationBuilder)
    {
        consumerConfigurationBuilder.Configure(configure =>
        {
            configure.SetTopicDeadLetter(TestConstant.TOPIC_DLQ_FAKE);
        });
    }
}
