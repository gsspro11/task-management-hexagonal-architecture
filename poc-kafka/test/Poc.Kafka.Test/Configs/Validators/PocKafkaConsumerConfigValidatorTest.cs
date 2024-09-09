using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Settings;
using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;
using Poc.Kafka.Test.CommonTests;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Configs.Validators;

public class PocKafkaConsumerConfigValidatorTest
{

    private readonly PocKafkaConsumerConfig _consumerConfig;
    public PocKafkaConsumerConfigValidatorTest() =>
        _consumerConfig = CreateConsumerConfig();

    [Fact]
    public void Validate_WhenValidConfigs_ValidatesSuccessfully()
    {
        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WhenNameIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'consumerConfig.Name')";

        _consumerConfig.SetName(null!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData("XXXXXXXX", null!)]
    [InlineData(null!, "XXXXXXXX")]
    public void Validate_GivenInvalidCredentials_ThrowsArgumentException(string username, string password)
    {
        string expectedErrorMessage = "Username and Password are required.";

        _consumerConfig.SetCredentials(username, password);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenBootstrapServersIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'consumerConfig.BootstrapServers')";

        _consumerConfig.SetBootstrapServers(null!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenTopicsIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'consumerConfig.Topics')";

        _consumerConfig.SetTopics(null!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate__WhenTopicsIsEmpty_ThrowsArgumentException()
    {
        string expectedErrorMessage = "No topics have been configured. Please specify at least one topic in the Topics property.";

        _consumerConfig.SetTopics(new List<PocTopicSettings>());

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenGroupIdIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'consumerConfig.GroupId')";

        _consumerConfig.SetGroupId(null!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenEnableRetryTopicConsumerAndTopicRetryIsNull_ThrowsArgumentException()
    {
        string expectedErrorMessage = "When EnableRetryTopicConsumer is enabled, the value of TopicRetry property is required.";

        _consumerConfig.SetEnableRetryTopicConsumer();
        _consumerConfig.SetTopicRetry(null!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ConsumerConstant.MIN_RETRY_DELAY_MS - 1)]
    [InlineData(ConsumerConstant.MAX_RETRY_DELAY_MS + 1)]
    public void Validate_GivenRetryDelayMsOutOfRange_ThrowsArgumentException(int retryDelayMs)
    {
        // Arrange

        _consumerConfig.SetEnableRetryTopicConsumer();
        _consumerConfig.SetTopicRetry("topic-retry");
        _consumerConfig.SetRetryDelayMs(retryDelayMs);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));
        Assert.Contains($"When {nameof(_consumerConfig.EnableRetryTopicConsumer)} is enabled, the retry delay must be within the specified range of {ConsumerConstant.MIN_RETRY_DELAY_MS} ms to {ConsumerConstant.MAX_RETRY_DELAY_MS} ms.", exception.Message);
    }


    [Theory]
    [InlineData(ConsumerConstant.MIN_RETRY_LIMIT - 1)] 
    [InlineData(ConsumerConstant.MAX_RETRY_LIMIT + 1)] 
    public void Validate_GivenRetryLimitOutOfRange_ThrowsArgumentException(int retryLimit)
    {
        // Arrange
        _consumerConfig.SetEnableRetryTopicConsumer();
        _consumerConfig.SetTopicRetry(TestConstant.TOPIC_RETRY_FAKE);
        _consumerConfig.SetRetryLimit(retryLimit);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));
        Assert.Contains($"When {nameof(_consumerConfig.EnableRetryTopicConsumer)} is enabled, the retry limit must be within the specified range of {ConsumerConstant.MIN_RETRY_LIMIT} to {ConsumerConstant.MAX_RETRY_LIMIT}.", exception.Message);
    }

    [Theory]
    [InlineData(ConsumerConstant.MIN_MAX_CONCURRENT_MESSAGES - 1)] 
    [InlineData(ConsumerConstant.MAX_MAX_CONCURRENT_MESSAGES_LIMIT + 1)] 
    public void Validate_GivenMaxConcurrentMessagesOutOfRange_ThrowsArgumentException(int maxConcurrentMessages)
    {
        // Arrange
        _consumerConfig.SetEnableRetryTopicConsumer();
        _consumerConfig.SetTopicRetry(TestConstant.TOPIC_RETRY_FAKE);
        _consumerConfig.SetMaxConcurrentMessages(maxConcurrentMessages);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));
        Assert.Contains($"When {nameof(_consumerConfig.EnableRetryTopicConsumer)} is enabled, the maximum number of concurrent messages must be within the specified range of {ConsumerConstant.MIN_MAX_CONCURRENT_MESSAGES} to {ConsumerConstant.MAX_MAX_CONCURRENT_MESSAGES_LIMIT}.", exception.Message);
    }


    [Fact]
    public void Validate_WhenEnableRetryTopicConsumerWithEnableAutoCommitTrue_ThrowsArgumentException()
    {
        // Arrange
        _consumerConfig.SetEnableRetryTopicConsumer();
        _consumerConfig.SetTopicRetry(TestConstant.TOPIC_RETRY_FAKE);
        _consumerConfig.SetEnableAutoCommit();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));
        Assert.Contains($"When {nameof(_consumerConfig.EnableRetryTopicConsumer)} is enabled, {nameof(_consumerConfig.EnableAutoCommit)} must be false.", exception.Message);
    }


    [Fact]
    public void Validate_WhenRetryTopicIsDuplicateOfSubscribedTopic_ThrowsArgumentException()
    {
        string expectedErrorMessage = "TopicRetry must be unique and cannot be one of the subscribed topics.";

        _consumerConfig.SetTopic(new PocTopicSettings { Topic = "topic-duplicated" });
        _consumerConfig.SetTopicRetry("topic-duplicated");

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData("topic-duplicated", "topic-retry", "topic-duplicated")]
    [InlineData("topic", "topic-duplicated", "topic-duplicated")]
    public void Validate_GivenDeadLetterTopicIsDuplicateOfSubscribedTopic_ThrowsArgumentException(string topic, string topicRetry, string topicDeadLetter)
    {
        string expectedErrorMessage = "TopicDeadLetter must be unique, cannot be one of the subscribed topics, and cannot be the same as TopicRetry.";

        _consumerConfig.SetTopic(new PocTopicSettings { Topic = topic });
        _consumerConfig.SetTopicRetry(topicRetry);
        _consumerConfig.SetTopicDeadLetter(topicDeadLetter);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenSubscribedTopicsAreDuplicated_ThrowsArgumentException()
    {
        string expectedErrorMessage = "Duplicate topic configurations detected for: topic1. Each topic must be unique.";

        var topics = new List<PocTopicSettings>()
        {
            new() { Topic = "topic1" },
            new() { Topic = "topic2" },
            new() { Topic = "topic1" },
            new() { Topic = "topic3" }
        };

        _consumerConfig.SetTopics(topics);
        _consumerConfig.SetTopicRetry("topic-retry");
        _consumerConfig.SetTopicDeadLetter("topic-dead-letter");

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenFetchWaitMaxMsIsNegative_ThrowsArgumentException()
    {
        string expectedErrorMessage = "The value of FetchWaitMaxMs must be non-negative.";

        _consumerConfig.SetFetchWaitMaxMs(-1!);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ConsumerConstant.MIN_FETCH_MIN_BYTES - 1)]
    [InlineData(ConsumerConstant.MAX_FETCH_MIN_BYTES + 1)]
    public void Validate_GivenFetchMinBytesIsOutOfRange_ThrowsArgumentException(int fetchMinBytes)
    {
        string expectedErrorMessage = $"The value of FetchMinBytes must be between {ConsumerConstant.MIN_FETCH_MIN_BYTES} and {ConsumerConstant.MAX_FETCH_MIN_BYTES}. Current value: {fetchMinBytes}.";

        _consumerConfig.SetFetchMinBytes(fetchMinBytes);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ConsumerConstant.MIN_FETCH_MAX_BYTES - 1)]
    [InlineData(ConsumerConstant.MAX_FETCH_MAX_BYTES + 1)]
    public void Validate_GivenFetchMaxBytesIsOutOfRange_ThrowsArgumentException(int fetchMaxBytes)
    {
        string expectedErrorMessage = $"The value of FetchMaxBytes must be between {ConsumerConstant.MIN_FETCH_MAX_BYTES} and {ConsumerConstant.MAX_FETCH_MAX_BYTES}. Current value: {fetchMaxBytes}.";

        _consumerConfig.SetFetchMaxBytes(fetchMaxBytes);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ConsumerConstant.MIN_PARTITION_FETCH_BYTES - 1)]
    [InlineData(ConsumerConstant.MAX_PARTITION_FETCH_BYTES + 1)]
    public void Validate_GivenMaxPartitionFetchBytesIsOutOfRange_ThrowsArgumentException(int maxPartitionFetchBytes)
    {
        string expectedErrorMessage = $"The value of MaxPartitionFetchBytes must be between {ConsumerConstant.MIN_PARTITION_FETCH_BYTES} and {ConsumerConstant.MAX_PARTITION_FETCH_BYTES}. Current value: {maxPartitionFetchBytes}.";

        _consumerConfig.SetMaxPartitionFetchBytes(maxPartitionFetchBytes);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }


    [Fact]
    public void Validate_WhenEnablePartitionEof_ThrowsArgumentException()
    {
        int delayPartitionEofMs = ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS - 1;
        string expectedErrorMessage = $"When EnablePartitionEof is enabled, the value of DelayPartitionEofMs property must be greater than or equal to {ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS}. Current value: {delayPartitionEofMs}.";

        _consumerConfig.SetDelayPartitionEofMs(delayPartitionEofMs);

        Exception ex = Record.Exception(() => PocKafkaConsumerConfigValidator.Validate(_consumerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    private static PocKafkaConsumerConfig CreateConsumerConfig()
    {
        var topics = new List<PocTopicSettings>()
        {
            new() { Topic = "topic1" },
            new() { Topic = "topic2" }
        };

        var consumerConfig = new PocKafkaConsumerConfig();
        consumerConfig.SetName("unique-consumer-name");
        consumerConfig.SetBootstrapServers("localhost:9092");
        consumerConfig.SetCredentials(username: "XXXXXXXX", password: "XXXXXXXX");
        consumerConfig.SetTopics(topics);
        consumerConfig.SetGroupId("group-id");
        consumerConfig.SetDelayPartitionEofMs(1000);
        return consumerConfig;
    }
}
