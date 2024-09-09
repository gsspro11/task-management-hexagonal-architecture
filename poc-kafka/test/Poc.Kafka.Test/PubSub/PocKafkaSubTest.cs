using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Managers;
using Poc.Kafka.PubSub;
using Poc.Kafka.Results;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Utilities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.PubSub;

public class PocKafkaSubTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<IRetryConsumerManager<string, string>> _mockRetryManager;
    private readonly Mock<IConsumerManager<string, string>> _mockConsumerManager;
    private readonly Mock<IConsumerConfiguration<string, string>> _mockConsumerConfiguration;

    private readonly PocKafkaSub<string, string> _sut;

    public PocKafkaSubTest()
    {
        _mockLogger = new();
        _mockRetryManager = new();
        _mockConsumerManager = new();
        _mockConsumerConfiguration = new();


        SetupConsumerConfiguration();

        _sut = InitializeSut();
    }

    [Fact]
    public async Task ConsumeAsync_WhenDisposed_DisposesResources()
    {
        //Act
        await _sut.DisposeAsync();

        //Assert
        VerifyDisposes();
    }

    [Fact]
    public async Task ConsumeAsync_WhenRetryTopicSubscriptionDisabled_OnlyProcessesMessagesFromPrimaryConsumer()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();

        SetupInitiateConsumeAsyncConsumerManager();

        //Act
        await _sut.ConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        //Assert
        VeryfyInitiateConsumeAsyncConsumerManager();
        VerifyInitiateConsumeAsyncRetryManager(times: Times.Never());
        VerifyDisposes();
    }

    [Theory]
    [InlineData(typeof(OperationCanceledException))]
    [InlineData(typeof(InvalidOperationException))]
    public async Task ConsumeAsync_WhenOperationCanceledException_DisposesResources(Type exceptionType)
    {
        // Arrange
        _mockConsumerManager
                     .Setup(x => x.InitiateConsumeAsync(
                         It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                         It.IsAny<CancellationToken>()))
                     .ThrowsAsync((Exception)Activator.CreateInstance(exceptionType)!);

        using var cancellationTokenSource = new CancellationTokenSource();

        //Act
        await _sut.ConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        //Assert
        VeryfyInitiateConsumeAsyncConsumerManager();
        VerifyDisposes();
    }

    [Fact]
    public async Task ConsumeAsync_WhenRetryTopicSubscriptionEnabled_ProcessesMessagesFromBothConsumers()
    {
        // Arrange
        SetupConsumerConfiguration(builder =>
        {
            builder.Configure(configure =>
            {
                configure.SetEnableRetryTopicConsumer();
                configure.SetTopicRetry(TestConstant.TOPIC_RETRY_FAKE);
            });
        });

        SetupInitiateConsumeAsyncRetryManager();
        SetupInitiateConsumeAsyncConsumerManager();

        using var cancellationTokenSource = new CancellationTokenSource();

        //Act
        await _sut.ConsumeAsync(_ => Task.CompletedTask, cancellationTokenSource.Token);

        //Assert
        VeryfyInitiateConsumeAsyncConsumerManager();
        VerifyInitiateConsumeAsyncRetryManager(times: Times.Once());
    }

    private PocKafkaSub<string, string> InitializeSut() => new(
            logger: _mockLogger.Object,
            retryManager: _mockRetryManager.Object,
            consumerManager: _mockConsumerManager.Object,
            consumerConfiguration: _mockConsumerConfiguration.Object);
    private void SetupConsumerConfiguration(Action<ConsumerConfigurationBuilder<string, string>> configure)
    {
        var consumerConfiguration = ConsumerConfigurationUtility.CreateConsumerConfiguration(configure);
        SetupConsumerConfiguration(consumerConfiguration);
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
    private void SetupInitiateConsumeAsyncRetryManager()
    {
        _mockRetryManager
                    .Setup(x => x.InitiateConsumeAsync(
                        It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
    }
    private void SetupInitiateConsumeAsyncConsumerManager()
    {
        _mockConsumerManager
            .Setup(x => x.InitiateConsumeAsync(
                It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }
    private void VerifyInitiateConsumeAsyncRetryManager(Times times)
    {
        _mockRetryManager
                   .Verify(x => x.InitiateConsumeAsync(
                       It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                       It.IsAny<CancellationToken>()), times);
    }
    private void VeryfyInitiateConsumeAsyncConsumerManager()
    {
        _mockConsumerManager
            .Verify(x => x.InitiateConsumeAsync(
                It.IsAny<Func<PocConsumeResult<string, string>, Task>>(),
                It.IsAny<CancellationToken>()), times: Times.Once);
    }
    private void VerifyDisposes()
    {
        _mockRetryManager
            .Verify(x => x.DisposeAsync(), times: Times.Once);

        _mockConsumerManager
            .Verify(x => x.DisposeAsync(), times: Times.Once);
    }

}
