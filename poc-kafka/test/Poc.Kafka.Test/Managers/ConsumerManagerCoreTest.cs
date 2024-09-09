using Poc.Kafka.Common;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Common.Settings;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.Providers;
using Poc.Kafka.Results;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Poc.Kafka.Test.CommonTests.Fakes;
using Poc.Kafka.Test.CommonTests.Utilities;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.Managers;
public class ConsumerManagerCoreTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<ITimeProvider> _mockTimeProvider;
    private readonly Mock<IDelayService> _mockDelayService;
    private readonly Mock<IKafkaConsumerFactory> _mockConsumerFactory;
    private readonly Mock<IKafkaProducerFactory> _mockProducerFactory;
    private readonly Mock<IConsumerConfiguration<string, string>> _mockConsumerConfiguration;
    private readonly Mock<IConsumer<string, string>> _mockConsumer;
    private readonly Mock<IProducer<string, string>> _mockProducer;
    private readonly ConsumerManagerCore<string, string> _sut;

    private readonly Message<string, string> _fakeMessage =
        KafkaMessageFactory.CreateKafkaMessage(value: TestConstant.VALUE_FAKE, key: TestConstant.KEY_FAKE, []);

    private readonly ConsumeResult<string, string> _fakeConsumeResult;
    public ConsumerManagerCoreTest()
    {
        _mockLogger = new();
        _mockTimeProvider = new();
        _mockDelayService = new();
        _mockConsumerFactory = new();
        _mockProducerFactory = new();
        _mockConsumerConfiguration = new();
        _mockConsumer = new();
        _mockProducer = new();

        _fakeConsumeResult = CreateConsumeResult(topic: TestConstant.TOPIC_FAKE);

        SetupUtcNow();
        SetupConsumerConfiguration();
        SetupConsume(_fakeConsumeResult);
        SetupCreateConsumer();
        SetupCreateProducer();
        SetupCommit();

        _sut = InitializeSut();
    }

    [Fact]
    public void ConsumeMessage_WhenCustomAvroDeserializersProvided_ProcessSuccessfully()
    {
        // Arrange
        var eventFake = AvroRecordFake.Create();
        var message = KafkaMessageFactory.CreateKafkaMessage(value: eventFake, key: TestConstant.KEY_FAKE, headers: []);
        var consumeResult = ConsumeResultFactory<string, AvroRecordFake>.CreateConsumeResult(
            topic: TestConstant.TOPIC_FAKE,
            message);

        var mockSchemaRegistry = new Mock<ISchemaRegistryClient>();

        var mockConsumer = new Mock<IConsumer<string, AvroRecordFake>>();
        mockConsumer
           .Setup(x => x.Consume(CancellationToken.None))
           .Returns(consumeResult);

        mockConsumer
           .Setup(x => x.Commit(
               It.IsAny<ConsumeResult<string, AvroRecordFake>>()));

        _mockConsumerFactory
          .Setup(x => x.CreateConsumer(It.IsAny<IConsumerConfiguration<string, AvroRecordFake>>()))
          .Returns(mockConsumer.Object);

        var consumerConfiguration = ConsumerConfigurationUtility.CreateConsumerConfiguration<string, AvroRecordFake>(builder =>
        {
            builder.WithSerialization(configure =>
            {
                configure.SetKeyDeserializer(new AvroDeserializer<string>(mockSchemaRegistry.Object).AsSyncOverAsync());
                configure.SetValueDeserializer(new AvroDeserializer<AvroRecordFake>(mockSchemaRegistry.Object).AsSyncOverAsync());

            });
        });

        var mockConsumerConfiguration = new Mock<IConsumerConfiguration<string, AvroRecordFake>>();
        mockConsumerConfiguration
         .Setup(m => m.ConsumerConfig)
            .Returns(consumerConfiguration.ConsumerConfig);

        mockConsumerConfiguration
           .Setup(m => m.SerializersConfig)
           .Returns(consumerConfiguration.SerializersConfig);


        var sut = new ConsumerManagerCore<string, AvroRecordFake>(
           logger: _mockLogger.Object,
           timeProvider: _mockTimeProvider.Object,
           delayService: _mockDelayService.Object,
           consumerFactory: _mockConsumerFactory.Object,
           producerFactory: _mockProducerFactory.Object,
           consumerConfiguration: mockConsumerConfiguration.Object);

        // Act
        sut.ConsumeMessage(CancellationToken.None);

        // Assert
        mockConsumer.Verify(x => x.Consume(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Subscribe_WhenValidTopic_SubscribesToTheSpecifiedTopic()
    {
        // Act
        _sut.Subscribe([TestConstant.TOPIC_FAKE]);

        // Assert  
        _mockConsumer.Verify(x => x.Subscribe(It.Is<IEnumerable<string>>(y => y.Contains(TestConstant.TOPIC_FAKE))), Times.Once);
        _mockConsumer.Verify(x => x.Assign(It.IsAny<IEnumerable<TopicPartition>>()), Times.Never);
    }

    [Fact]
    public void Seek_WhenValidTopicPartitionOffset_SeeksMessageForGivenTopicPartitionOffset()
    {
        // Act
        var topicPartitionOffset = new TopicPartitionOffset(
            new TopicPartition(topic: TestConstant.TOPIC_FAKE, partition: 0),
            offset: 1);

        _sut.Seek(topicPartitionOffset);

        // Assert  
        VerifySeek(topicPartitionOffset);
    }

    [Fact]
    public void Seek_WhenThrowsException_LogsError()
    {
        // Act
        var topicPartitionOffset = new TopicPartitionOffset(
            new TopicPartition(topic: TestConstant.TOPIC_FAKE, partition: 0),
            offset: 1);

        _mockConsumer
            .Setup(x => x.Seek(It.IsAny<TopicPartitionOffset>()))
            .Throws(new KafkaException(new Error(ErrorCode.Local_Outdated)));

        _sut.Seek(topicPartitionOffset);

        // Assert  
        VerifySeek(topicPartitionOffset);
    }

    [Fact]
    public void ConfigureTopicsSubscription_GivenValidTopics_SubscribesAndAssignsCorrectly()
    {
        // Arrange
        var topicSettings = new List<PocTopicSettings>
        {
            new() { Topic = "SubscribeTopic1" },
            new() { Topic = "AssignTopic1", Partitions = [0] },
            new() { Topic = "SubscribeTopic2" },
            new() { Topic = "AssignTopic2", Partitions = [0,1] },
            new() { Topic = "SubscribeTopic3", Partitions = [] }
        };

        SetupConsumerConfiguration(builder =>
        {
            builder.Configure(configure =>
            {
                configure.SetTopics(topicSettings);
            });
        });

        // Act
        _sut.ConfigureTopicsSubscription();

        // Assert  
        _mockConsumer.Verify(x => x.Subscribe(It.Is<IEnumerable<string>>(y => y.Contains("SubscribeTopic1")
            && y.Contains("SubscribeTopic2") && y.Contains("SubscribeTopic3"))), Times.Once);

        _mockConsumer.Verify(x => x.Assign(It.Is<IEnumerable<TopicPartition>>(y =>
            y.Any(partition => partition.Topic == "AssignTopic1" && partition.Partition.Value == 0) &&
            y.Count(partition => partition.Topic == "AssignTopic2" && (partition.Partition.Value == 0 || partition.Partition.Value == 1)) == 2)),
            Times.Once);
    }

    [Fact]
    public void ConsumeMessage_WhenConsumerCreationFails_ThrowsException()
    {
        // arrange
        string expectedExceptionMessage = "Error creating Kafka consumer.";

        _mockConsumerFactory
            .Setup(x => x.CreateConsumer(It.IsAny<IConsumerConfiguration<string, string>>()))
            .Throws(new InvalidOperationException(expectedExceptionMessage));

        // act && assert
        var exception = Assert.Throws<InvalidOperationException>(() => _sut.ConsumeMessage(CancellationToken.None));
        Assert.Contains(expectedExceptionMessage, exception.Message);
    }

    [Fact]
    public void ConsumeMessage_WhenMessageIsNull_ReturnsNull()
    {
        // Arrange
        SetupConsume(null!);

        // Act
        var consumeResult = _sut.ConsumeMessage(CancellationToken.None);

        // Assert
        Assert.Null(consumeResult);

        VerifyConsume(Times.Once());
    }

    [Fact]
    public void ConsumeMessage_WhenValidMessage_ConsumesMessage()
    {
        // Act
        var consumeResult = _sut.ConsumeMessage(CancellationToken.None);

        // Assert
        Assert.Equivalent(_fakeConsumeResult, consumeResult);

        VerifyConsume(Times.Once());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenEofAndDelayPartitionEofMsGreaterThanZero_ProcessEofSuccessfully()
    {
        //Arrange
        int delayPartitionEofMs = 500;

        var consumeResultEof = ConsumeResultFactory<string, string>.CreateConsumeResult(
            topic: TestConstant.TOPIC_FAKE,
            isPartitionEof: true);

        SetupConsumerConfiguration(builder =>
        {
            builder.Configure(config =>
            {
                config.SetDelayPartitionEofMs(delayPartitionEofMs);
            });
        });

        SetupTaskDelay(delayPartitionEofMs);

        // Act
        await _sut.ProcessMessageAsync(consumeResultEof, OnMessageReceived, CancellationToken.None);

        // Assert
        VerifyTaskDelay(delayPartitionEofMs, Times.Once());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenEofAndDelayPartitionEofMsLessThanZero_ProcessesEofSuccessfully()
    {
        //Arrange
        int delayPartitionEofMs = 500;

        var consumeResultEof = ConsumeResultFactory<string, string>.CreateConsumeResult(
            topic: TestConstant.TOPIC_FAKE,
            isPartitionEof: true);

        // Act
        await _sut.ProcessMessageAsync(consumeResultEof, OnMessageReceived, CancellationToken.None);

        // Assert
        VerifyTaskDelay(delayPartitionEofMs, Times.Never());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenfCancellationTokenRequested_ThrowsOperationCanceledException()
    {
        //Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceived, cancellationTokenSource.Token));
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenAutoCommitDisabled_ProcessMessageSuccessfully()
    {
        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceived, CancellationToken.None);

        // Assert
        VerifyCommit(Times.Once());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenIsConcurrentlyProcess_ProcessMessageSuccessfully()
    {
        //Arrange
        int maxConcurrentMessages = 5;  

        // Act
        SetupConsumerConfiguration(builder =>
        {
            builder.Configure(configure =>
            {
                configure.SetMaxConcurrentMessages(maxConcurrentMessages);
            });
        });

        for (int i = 0; i < maxConcurrentMessages; i++)
        {
            await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceived, CancellationToken.None);
        }

        // Assert
        VerifyCommit(Times.Exactly(maxConcurrentMessages));
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenAutoCommitEnabled_ProcessMessageSuccessfully()
    {
        SetupConsumerConfiguration(builder =>
        {
            builder.Configure(configure =>
            {
                configure.SetEnableAutoCommit();
            });
        });

        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceived, CancellationToken.None);

        // Assert
        VerifyCommit(times: Times.Never());
    }

    [Fact]
    public async Task ConsumeAndProcessMessageAsync_WhenTryAgainInvokeExpontaneous_ProcessMessageAndPublishToRetryTopic()
    {
        //Arrange
        SetupConsumerConfiguration(builder =>
        {
            ConsumerConfigurationUtility.ConfigureRetryAndDeadLetter(builder);
        });

        var deliveryResult = CreateDeliveryResult(topic: TestConstant.TOPIC_RETRY_FAKE);
        SetupProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, deliveryResult);

        static Task onMessageReceived(PocConsumeResult<string, string> pocConsumeResult)
        {
            pocConsumeResult.TryAgain();
            return Task.CompletedTask;
        }

        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, onMessageReceived, CancellationToken.None);

        // Assert
        VerifyCommit(times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, times: Times.Never());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenSkipRetryAndSendToDeadLetterInvokeExpontaneous_ProcessMessageAndPublishToDlqTopic()
    {
        //Arrange
        SetupConsumerConfiguration(builder =>
        {
            ConsumerConfigurationUtility.ConfigureDeadLetter(builder);
        });

        var deliveryResult = CreateDeliveryResult(topic: TestConstant.TOPIC_DLQ_FAKE);
        SetupProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, deliveryResult);

        static Task onMessageReceived(PocConsumeResult<string, string> pocConsumeResult)
        {
            pocConsumeResult.SkipRetryAndSendToDeadLetter();
            return Task.CompletedTask;
        }

        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, onMessageReceived, CancellationToken.None);

        // Assert
        VerifyCommit(times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, times: Times.Never());
        VerifyProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, times: Times.Once());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenOnMessageReceivedThrowsException_HandleRetryFlow()
    {
        SetupConsumerConfiguration(builder =>
        {
            ConsumerConfigurationUtility.ConfigureRetryAndDeadLetter(builder);
        });

        var deliveryResult = CreateDeliveryResult(topic: TestConstant.TOPIC_RETRY_FAKE);
        SetupProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, deliveryResult);


        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceivedWithThrowsException, CancellationToken.None);

        // Assert
        VerifyCommit(times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, times: Times.Never());
    }

    [Fact]
    public async Task WhenOnMessageReceivedThrowsException_EntersRetryFlowFailsToPublishToRetryThenSucceedsInDlq()
    {
        int retryLimit = 2;

        SetupConsumerConfiguration(builder =>
        {
            ConsumerConfigurationUtility.ConfigureRetryAndDeadLetter(builder);

            builder.Configure(config =>
            {
                config.SetRetryLimit(retryLimit);
            });
        });

        SetupProduceAsyncThrowsException(topic: TestConstant.TOPIC_RETRY_FAKE);

        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceivedWithThrowsException, CancellationToken.None);

        // Assert
        VerifyCommit(times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, times: Times.Exactly(retryLimit));
        VerifyProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, times: Times.Once());
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenOnMessageReceivedThrowsException_SkipsRetryAndPublishesToDlqSuccessfully()
    {
        SetupConsumerConfiguration(builder =>
        {
            ConsumerConfigurationUtility.ConfigureDeadLetter(builder);
        });

        var deliveryResult = CreateDeliveryResult(topic: TestConstant.TOPIC_DLQ_FAKE);
        SetupProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, deliveryResult);

        // Act
        await _sut.ProcessMessageAsync(_fakeConsumeResult, OnMessageReceivedThrowExceptionSkipRetryAndSendToDlq, CancellationToken.None);

        await _sut.DisposeAsync();

        // Assert
        VerifyCommit(times: Times.Once());
        VerifyProduceAsync(topic: TestConstant.TOPIC_RETRY_FAKE, times: Times.Never());
        VerifyProduceAsync(topic: TestConstant.TOPIC_DLQ_FAKE, times: Times.Once());

        _mockConsumer.Verify(m => m.Close(), Times.Once);
        _mockConsumer.Verify(m => m.Dispose(), Times.Once);
        _mockProducer.Verify(m => m.Dispose(), Times.Once);

    }
    private ConsumerManagerCore<string, string> InitializeSut()
    {
        var consumerManager = new ConsumerManagerCore<string, string>(
            logger: _mockLogger.Object,
            timeProvider: _mockTimeProvider.Object,
            delayService: _mockDelayService.Object,
            consumerFactory: _mockConsumerFactory.Object,
            producerFactory: _mockProducerFactory.Object,
            consumerConfiguration: _mockConsumerConfiguration.Object);

        return consumerManager;
    }

    private DeliveryResult<string, string> CreateDeliveryResult(string topic)
    {
        return DeliveryResultFactory<string, string>.CreateDeliveryResult(
           topic,
           message: _fakeMessage);
    }

    static Task OnMessageReceived<TKey, TValue>(PocConsumeResult<TKey, TValue> _) => Task.CompletedTask;

    static Task OnMessageReceivedWithThrowsException<TKey, TValue>(PocConsumeResult<TKey, TValue> _) =>
        throw new InvalidOperationException("Invalid operation exception.");

    static Task OnMessageReceivedThrowExceptionSkipRetryAndSendToDlq<TKey, TValue>(
        PocConsumeResult<TKey, TValue> pocConsumeResult)
    {
        try
        {
            throw new InvalidOperationException("Invalid operation exception.");
        }
        catch (Exception)
        {
            pocConsumeResult.SkipRetryAndSendToDeadLetter();
            throw;
        }
    }
    private ConsumeResult<string, string> CreateConsumeResult(string topic)
    {
        return ConsumeResultFactory<string, string>.CreateConsumeResult(
                    topic,
                    message: _fakeMessage);
    }

    private void SetupUtcNow(DateTimeOffset expectedTime = default)
    {
        if (expectedTime == default)
            expectedTime = DateTimeOffset.Now;

        _mockTimeProvider
            .Setup(m => m.UtcNow)
            .Returns(expectedTime)
            .Verifiable();
    }
    private void SetupTaskDelay(int millisecondsDelay)
    {
        _mockDelayService
            .Setup(m => m.Delay(It.Is<int>(x => x == millisecondsDelay), It.IsAny<CancellationToken>()))
            .Verifiable();
    }
    private void SetupConsumerConfiguration(Action<ConsumerConfigurationBuilder<string, string>> configure)
    {
        var consumerConfiguration = ConsumerConfigurationUtility.CreateConsumerConfiguration(configure);
        SetupConsumerConfiguration(consumerConfiguration);
    }
    private void SetupConsumerConfiguration(IConsumerConfiguration<string, string>? consumerConfiguration = null)
    {
        consumerConfiguration ??=
            ConsumerConfigurationUtility.CreateConsumerConfigurationBuilder<string, string>().Build();

        _mockConsumerConfiguration
        .Setup(m => m.ConsumerConfig)
           .Returns(consumerConfiguration.ConsumerConfig);

        _mockConsumerConfiguration
           .Setup(m => m.SerializersConfig)
           .Returns(consumerConfiguration.SerializersConfig);
    }
    private void SetupConsume(ConsumeResult<string, string> consumeResult)
    {
        _mockConsumer.Setup(x => x.Consume(It.IsAny<CancellationToken>()))
              .Returns(consumeResult);
    }
    private void SetupCreateConsumer()
    {
        _mockConsumerFactory
            .Setup(x => x.CreateConsumer(It.IsAny<IConsumerConfiguration<string, string>>()))
            .Returns(_mockConsumer.Object);
    }
    private void SetupProduceAsync(string topic, DeliveryResult<string, string> deliveryResult)
    {
        _mockProducer
            .Setup(x => x.ProduceAsync(
                It.Is<string>(t => t == topic),
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);
    }
    private void SetupProduceAsyncThrowsException(string topic)
    {
        _mockProducer
            .Setup(x => x.ProduceAsync(
                It.Is<string>(t => t == topic),
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Simulated publishing failure"));
    }
    private void SetupCreateProducer()
    {
        _mockProducerFactory
            .Setup(x => x.CreateProducer(It.IsAny<IProducerConfiguration<string, string>>()))
            .Returns(_mockProducer.Object);
    }
    private void SetupCommit()
    {
        _mockConsumer
            .Setup(x => x.Commit(
                It.IsAny<ConsumeResult<string, string>>()))
            .Verifiable();

    }
    private void VerifyConsume(Times times)
    {
        _mockConsumer
         .Verify(x => x.Consume(
             It.IsAny<CancellationToken>()), times);

    }
    private void VerifyCommit(Times times)
    {
        _mockConsumer
            .Verify(x => x.Commit(
                It.IsAny<ConsumeResult<string, string>>()),
            times);
    }
    private void VerifyTaskDelay(int delayPartitionEofMs, Times times)
    {
        _mockDelayService
            .Verify(m => m.Delay(
                It.Is<int>(x => x == delayPartitionEofMs),
                It.IsAny<CancellationToken>()), times);
    }
    private void VerifyProduceAsync(string topic, Times times)
    {
        _mockProducer
            .Verify(x => x.ProduceAsync(
                It.Is<string>(t => t == topic),
                It.IsAny<Message<string, string>>(),
                It.IsAny<CancellationToken>()), times);
    }

    private void VerifySeek(TopicPartitionOffset topicPartitionOffset)
    {
        _mockConsumer
            .Verify(x => x.Seek(
                It.Is<TopicPartitionOffset>(tpo => tpo.Equals(topicPartitionOffset))), Times.Once);
    }

}
