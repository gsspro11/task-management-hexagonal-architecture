using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.Providers;
using Poc.Kafka.Results;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Poc.Kafka.Test.CommonTests.Utilities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Poc.Kafka.Test.Managers;
public class RetryConsumerManagerTest
{
    private readonly Mock<IDelayService> _mockDelayService;
    private readonly Mock<IRetryProvider> _mockRetryProvider;
    private readonly Mock<IConsumerManagerCore<string, string>> _mockConsumerManagerCore;
    private readonly Mock<IConsumerConfiguration<string, string>> _mockConsumerConfiguration;
    private readonly RetryConsumerManager<string, string> _sut;

    private readonly Message<string, string> _fakeMessage =
      KafkaMessageFactory.CreateKafkaMessage(value: TestConstant.VALUE_FAKE, key: TestConstant.KEY_FAKE, []);

    private readonly ConsumeResult<string, string> _consumeResult;

    public RetryConsumerManagerTest()
    {
        _mockDelayService = new();
        _mockRetryProvider = new();
        _mockConsumerManagerCore = new();
        _mockConsumerConfiguration = new();

        _consumeResult = ConsumeResultFactory<string, string>.CreateConsumeResult(
            topic: TestConstant.TOPIC_FAKE,
            message: _fakeMessage);

        SetupConsumeMessage();
        SetupConsumerConfiguration();

        _sut = InitializeSut();
    }

    [Fact]
    public async Task InitiateConsumeAsync_WhenRetryDelayExpires_ProcessMessageSuccess()
    {
        //Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        _mockConsumerManagerCore
          .Setup(x => x.ProcessMessageAsync(
              It.IsAny<ConsumeResult<string, string>>(),
              It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
              It.IsAny<CancellationToken>()))
            .Callback<ConsumeResult<string, string>, Func<PocConsumeResult<string, string>, Task>, CancellationToken>((_, _, _) =>
            {
                // Forces exit from the loop to conclude the test.
                cancellationTokenSource.Cancel();
            })
          .Returns(Task.CompletedTask);

        SetupIsRetryDelayExpired(true);

        // Act
        await _sut.InitiateConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        // Assert
        VerifySubscribe();
        VerifyIsRetryDelayExpired();
        VerifyConsumeMessage(Times.Once());
        VerifyProcessMessageAsync(Times.Once());
    }

    [Fact]
    public async Task InitiateConsumeAsync_WhenRetryDelayIsNotExpired_ProcessMessageSuccess()
    {
        //Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        _mockConsumerManagerCore
            .Setup(x => x.Seek(
                It.IsAny<TopicPartitionOffset>()))
              .Callback<TopicPartitionOffset>((topicPartitionOffset) =>
              {
                  // Forces exit from the loop to conclude the test.
                  cancellationTokenSource.Cancel();
              });


        SetupIsRetryDelayExpired(false);

        // Act
        await _sut.InitiateConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        await _sut.DisposeAsync();

        // Assert
        VerifySubscribe();
        VerifyIsRetryDelayExpired();
        VerifyConsumeMessage(Times.Once());
        VerifyProcessMessageAsync(Times.Never());
        VerifyWaitBeforeNextRetryAsync();
        VerifySeek();

        _mockConsumerManagerCore
            .Verify(x => x.DisposeAsync(), times: Times.Once);
    }


    private RetryConsumerManager<string, string> InitializeSut()
    {
        var retryConsumerManager = new RetryConsumerManager<string, string>(
            logger: NullLogger<IPocKafkaPubSub>.Instance,
            retryProvider: _mockRetryProvider.Object,
            delayService: _mockDelayService.Object,
            consumerCore: _mockConsumerManagerCore.Object,
            consumerConfiguration: _mockConsumerConfiguration.Object);

        return retryConsumerManager;
    }

    private void SetupConsumerConfiguration(IConsumerConfiguration<string, string>? consumerConfiguration = null)
    {
        consumerConfiguration ??= ConsumerConfigurationUtility.CreateConsumerConfigurationBuilder<string, string>()
            .Build();

        _mockConsumerConfiguration
        .Setup(m => m.ConsumerConfig)
           .Returns(consumerConfiguration.ConsumerConfig);

        _mockConsumerConfiguration
           .Setup(m => m.SerializersConfig)
           .Returns(consumerConfiguration.SerializersConfig);
    }

    private void SetupConsumeMessage()
    {
        _mockConsumerManagerCore
           .Setup(x => x.ConsumeMessage(
               It.IsAny<CancellationToken>()))
           .Returns(_consumeResult);
    }

    private void SetupIsRetryDelayExpired(bool value)
    {
        _mockRetryProvider
                    .Setup(x => x.IsRetryDelayExpired(It.IsAny<Headers>()))
                    .Returns(value);
    }

    private void VerifySubscribe()
    {
        _mockConsumerManagerCore
           .Verify(x => x.Subscribe(
               It.IsAny<IEnumerable<string>>()), times: Times.Once);
    }

    private void VerifyIsRetryDelayExpired()
    {
        _mockRetryProvider
                            .Verify(x => x.IsRetryDelayExpired(
                                It.IsAny<Headers>()), times: Times.Once);
    }

    private void VerifyConsumeMessage(Times times)
    {
        _mockConsumerManagerCore
                    .Verify(x => x.ConsumeMessage(
                        It.IsAny<CancellationToken>()), times);
    }

    private void VerifyProcessMessageAsync(Times times)
    {
        _mockConsumerManagerCore
                    .Verify(x => x.ProcessMessageAsync(
                        It.IsAny<ConsumeResult<string, string>>(),
                        It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                        It.IsAny<CancellationToken>()), times);
    }
    private void VerifyWaitBeforeNextRetryAsync()
    {
        _mockRetryProvider
                    .Verify(x => x.WaitBeforeNextRetryAsync(
                        It.IsAny<CancellationToken>()), times: Times.Once);
    }

    private void VerifySeek()
    {
        _mockConsumerManagerCore
            .Verify(x => x.Seek(
                It.IsAny<TopicPartitionOffset>()), times: Times.Once);
    }

}
