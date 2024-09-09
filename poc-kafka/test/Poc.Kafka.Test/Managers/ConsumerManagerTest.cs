using Poc.Kafka.Common;
using Poc.Kafka.Common.Constants;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.Results;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Poc.Kafka.Test.CommonTests.Utilities;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.Managers;
public class ConsumerManagerTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<IDelayService> _mockDelayService;
    private readonly Mock<IConsumerManagerCore<string, string>> _mockConsumerManagerCore;
    private readonly Mock<IConsumerConfiguration<string, string>> _mockConsumerConfiguration;
    private readonly ConsumerManager<string, string> _sut;

    private readonly Message<string, string> _fakeMessage =
      KafkaMessageFactory.CreateKafkaMessage(value: TestConstant.VALUE_FAKE, key: TestConstant.KEY_FAKE, []);

    private readonly ConsumeResult<string, string> _consumeResult;

    public ConsumerManagerTest()
    {
        _mockLogger = new();
        _mockDelayService = new();
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
    public async Task InitiateConsumeAsync_WhenMessageSuccessfullyProcessed_ProcessMessageSuccess()
    {
        //Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        _ = _mockConsumerManagerCore
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

        // Act
        await _sut.InitiateConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        // Assert
        VerifyConfigureTopicsSubscription();
        VerifyConsumeMessage(Times.Once());
        VerifyProcessMessageAsync(Times.Once());
    }


    [Fact]
    public async Task InitiateConsumeAsync_WhenMessageExceptionProcessedOccurred_ExponentialRetryAndBreaksConsumeLoop()
    {
        //Arrange
        int maxAttemptsOnError = ConsumerConstant.MAX_ATTEMPTS_ON_ERROR_CONSUMPTION;

        _mockDelayService
            .Setup(x => x.Delay(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockConsumerManagerCore
          .Setup(x => x.ProcessMessageAsync(
              It.IsAny<ConsumeResult<string, string>>(),
              It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
              It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Invalid operation exception."));

        // Act
        await _sut.InitiateConsumeAsync(_ => Task.CompletedTask, CancellationToken.None);

        // Assert
        VerifyConfigureTopicsSubscription();
        VerifyConsumeMessage(times: Times.Exactly(maxAttemptsOnError));
        VerifyProcessMessageAsync(times: Times.Exactly(maxAttemptsOnError));

        int expectedDelayCalls = maxAttemptsOnError - 1;
        _mockDelayService
            .Verify(x => x.Delay(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
                times: Times.Exactly(expectedDelayCalls));
    }

    [Fact]
    public async Task InitiateConsumeAsync_WhenOperationCanceledException_BreaksConsumeLoop()
    {
        //Arrange
        _mockConsumerManagerCore
         .Setup(x => x.ProcessMessageAsync(
             It.IsAny<ConsumeResult<string, string>>(),
             It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
             It.IsAny<CancellationToken>()))
           .ThrowsAsync(new OperationCanceledException());

        // Act
        await _sut.InitiateConsumeAsync(_ => Task.CompletedTask, CancellationToken.None);

        await _sut.DisposeAsync();

        // Assert
        VerifyConfigureTopicsSubscription();
        VerifyConsumeMessage(times: Times.Once());
        VerifyProcessMessageAsync(times: Times.Once());

        _mockConsumerManagerCore
            .Verify(x => x.DisposeAsync(), times: Times.Once);
    }

    private ConsumerManager<string, string> InitializeSut()
    {
        var consumerManager = new ConsumerManager<string, string>(
            logger: _mockLogger.Object,
            delayService: _mockDelayService.Object,
            consumerCore: _mockConsumerManagerCore.Object,
            consumerConfiguration: _mockConsumerConfiguration.Object);

        return consumerManager;
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
    private void VerifyConfigureTopicsSubscription() =>
        _mockConsumerManagerCore
        .Verify(x => x.ConfigureTopicsSubscription(), times: Times.Once);

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
}
