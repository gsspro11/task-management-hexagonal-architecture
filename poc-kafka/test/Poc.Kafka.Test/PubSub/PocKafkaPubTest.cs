using Poc.Kafka.Common;
using Poc.Kafka.Common.Exceptions;
using Poc.Kafka.Configurations;
using Poc.Kafka.Configurators;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.PubSub;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.PubSub;

public partial class PocKafkaPubTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<IProducerManager<string, string>> _mockProducerManager;
    private readonly Mock<IProducerConfiguration<string, string>> _mockProducerConfiguration;
    private readonly PocKafkaPub<string, string> _sut;

    private readonly Message<string, string> _fakeKafkaMessage =
       KafkaMessageFactory.CreateKafkaMessage(value: TestConstant.VALUE_FAKE, key: TestConstant.KEY_FAKE, headers: []);

    private readonly DeliveryResult<string, string> _fakeDeliveryResult;

    public PocKafkaPubTest()
    {
        _mockLogger = new();
        _mockProducerManager = new();
        _mockProducerConfiguration = new();

        _fakeDeliveryResult = DeliveryResultFactory<string, string>.CreateDeliveryResult(
            topic: TestConstant.TOPIC_FAKE,
            message: _fakeKafkaMessage);

        SetupProducerConfig();

        _sut = InitializeSut();
    }


    [Fact]
    public void ConsumeAsync_WhenDisposed_DisposesResources()
    {
        //Act
        _sut.Dispose();

        //Assert
        _mockProducerManager
            .Verify(x => x.Dispose(), times: Times.Once);
    }

    [Fact]
    public async Task SendAsync_WhenKeyValueAndHeadersProvided_ReturnsDeliveryResult()
    {
        // Arrange
        SetupSendMessageAsyncSuccess(_fakeDeliveryResult);

        // Act
        var deliveryResult = await _sut.SendAsync(
            value: TestConstant.VALUE_FAKE,
            key: TestConstant.KEY_FAKE,
            headers: []);

        // Assert
        Assert.Same(_fakeDeliveryResult, deliveryResult);
        VerifySendMessageAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task SendAsync_WhenTopicProvided_ReturnsDeliveryResult()
    {
        // Arrange
        SetupSendMessageAsyncSuccess(_fakeDeliveryResult);

        // Act
        var deliveryResult = await _sut.SendAsync(
            message: _fakeKafkaMessage,
            topic: TestConstant.TOPIC_FAKE,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Same(_fakeDeliveryResult, deliveryResult);
        VerifySendMessageAsyncCalled(Times.Once());
    }


    [Fact]
    public void Send_WhenTopicProvided_SendsMessageSuccessAndInvokesDeliveryHandler()
    {
        // Arrange
        SetupSendMessageWithDeliveryReportCallbackcSuccess(_fakeDeliveryResult);

        bool deliveryHandlerCalled = false;
        void deliveryHandler(DeliveryReport<string, string> deliveryReport)
        {
            Assert.Equal(ErrorCode.NoError, deliveryReport.Error.Code);
            deliveryHandlerCalled = true;
        }

        // Act
        _sut.Send(
            message: _fakeKafkaMessage,
            topic: TestConstant.TOPIC_FAKE,
            deliveryHandler: deliveryHandler);

        // Assert
        Assert.True(deliveryHandlerCalled);
        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Once());
    }

    [Fact]
    public void Send_WhenTopicNotProvided_SendsMessageSuccessAndInvokesDeliveryHandler()
    {
        // Arrange
        SetupSendMessageWithDeliveryReportCallbackcSuccess(_fakeDeliveryResult);

        bool deliveryHandlerCalled = false;
        void deliveryHandler(DeliveryReport<string, string> deliveryReport)
        {
            Assert.Equal(ErrorCode.NoError, deliveryReport.Error.Code);
            deliveryHandlerCalled = true;
        }

        // Act
        _sut.Send(
            value: TestConstant.VALUE_FAKE,
            key: TestConstant.KEY_FAKE,
            headers: [],
            deliveryHandler: deliveryHandler);

        // Assert
        Assert.True(deliveryHandlerCalled);
        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Once());
    }


    [Fact]
    public void Send_WhenError_InvokesDefaultDeliveryHandlerAndThrowsException()
    {
        // Arrange
        SetupSendMessageWithDeliveryReportCallbackcError(_fakeDeliveryResult);

        // Act
        Exception ex = Assert.Throws<KafkaDeliveryException>(() => _sut.Send(
            value: TestConstant.VALUE_FAKE,
            key: TestConstant.KEY_FAKE,
            headers: []));

        // Assert
        Assert.Equal(TestConstant.EXPECTED_ERROR_REASON_PARTIAL_MESSAGE, ex.Message);
        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Once());
    }

    [Fact]
    public void Send_WhenSuccessAndDeliveryHandlerNotProvided_InvokesDefaultDeliveryHandler()
    {
        // Arrange
        SetupSendMessageWithDeliveryReportCallbackcSuccess(_fakeDeliveryResult);

        // Act
        _sut.Send(
            value: TestConstant.VALUE_FAKE,
            key: TestConstant.KEY_FAKE,
            headers: []);

        // Assert
        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Once());
    }


    private PocKafkaPub<string, string> InitializeSut() =>
        new(_mockLogger.Object, _mockProducerManager.Object, _mockProducerConfiguration.Object);


    private void SetupProducerConfig()
    {
        var producerConfig = new Kafka.Configs.PocKafkaProducerConfig();
        producerConfig.SetName("UniqueProducerName");

        _mockProducerConfiguration
            .Setup(m => m.ProducerConfig)
                  .Returns(producerConfig);
    }
    private void SetupSendMessageAsyncSuccess(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducerManager
            .Setup(x => x.SendMessageAsync(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryResult);
    }
    private void SetupSendMessageWithDeliveryReportCallbackcSuccess(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<string>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()))
            .Callback<Message<string, string>, string, Action<DeliveryReport<string, string>>>((message, topic, handler) =>
            {
                var deliveryReport = CreateDeliveryReportSuccess(deliveryResult, message);
                handler(deliveryReport);
            });
    }
    private static DeliveryReport<string, string> CreateDeliveryReportSuccess(
        DeliveryResult<string, string> deliveryResult,
        Message<string, string> message) => new()
        {
            Topic = deliveryResult.Topic,
            Error = new Error(ErrorCode.NoError),
            Message = message
        };


    private void SetupSendMessageWithDeliveryReportCallbackcError(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<string>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()))
            .Callback<Message<string, string>, string, Action<DeliveryReport<string, string>>>((message, topic, handler) =>
            {
                var errorReport = CreateDeliveryReportError(deliveryResult, message);
                handler(errorReport);
            });
    }
    private static DeliveryReport<string, string> CreateDeliveryReportError(
        DeliveryResult<string, string> deliveryResult,
        Message<string, string> message) => new()
        {
            Topic = deliveryResult.Topic,
            Partition = new Partition(0),
            Offset = new Offset(5),
            Status = PersistenceStatus.NotPersisted,
            Error = new Error(ErrorCode.Local_Partial, "Partial message was received."),
            Message = message
        };

    private void VerifySendMessageAsyncCalled(Times times)
    {
        _mockProducerManager
            .Verify(x => x.SendMessageAsync(
                It.IsAny<Message<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), times);
    }
    private void VerifySendMessageAsyncWithTopicCalled(Times times)
    {
        _mockProducerManager
            .Verify(x => x.SendMessageAsync(
                It.IsAny<Message<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), times);
    }

    private void VerifySendMessageWithDeliveryReportCallbackCalled(Times times)
    {
        _mockProducerManager
            .Verify(x => x.SendMessage(
                It.IsAny<Message<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()), times);
    }
}
