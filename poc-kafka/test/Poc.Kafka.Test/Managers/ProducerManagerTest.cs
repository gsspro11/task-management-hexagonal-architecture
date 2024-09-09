using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Poc.Kafka.Test.CommonTests.Fakes;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.Managers;

public class ProducerManagerTest
{
    private readonly Mock<IProducer<string, string>> _mockProducer;
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<IKafkaProducerFactory> _mockProducerFactory;
    private readonly Mock<IProducerConfiguration<string, string>> _mockProducerConfiguration;
    private readonly ProducerManager<string, string> _sut;

    private readonly Message<string, string> _fakeMessage =
       KafkaMessageFactory.CreateKafkaMessage(value: TestConstant.VALUE_FAKE, key: TestConstant.KEY_FAKE, headers: []);

    public ProducerManagerTest()
    {
        _mockLogger = new();
        _mockProducerConfiguration = new();
        _mockProducerFactory = new();
        _mockProducer = new();

        SetupCreateProducer();
        SetupProducerConfiguration();

        _sut = InitializeSut();
    }

    [Fact]
    public void AllProxyMethods_WhenInvoked_InvokeCorrespondingThirdPartyLibMethods()
    {
        // Arrange
        var timeout = TimeSpan.FromSeconds(5);

        // Act
        _sut.Flush(timeout);
        _sut.InitTransactions(timeout);
        _sut.BeginTransaction();
        _sut.CommitTransaction();
        _sut.AbortTransaction();

        // Assert
        _mockProducer.Verify(p => p.Flush(timeout), Times.Once);
        _mockProducer.Verify(p => p.InitTransactions(timeout), Times.Once);
        _mockProducer.Verify(p => p.BeginTransaction(), Times.Once);
        _mockProducer.Verify(p => p.CommitTransaction(), Times.Once);
        _mockProducer.Verify(p => p.AbortTransaction(), Times.Once);
    }

    [Fact]
    public void SendMessageAsync_WhenCreateProducerFails_ThrowsException()
    {
        // Arrange
        string expectedErrorMessage = "Failed to create producer.";

        _mockProducerFactory
            .Setup(f => f.CreateProducer(It.IsAny<IProducerConfiguration<string, string>>()))
            .Throws(new InvalidOperationException(expectedErrorMessage));

        // Act & Assert
        Exception ex = Assert.Throws<InvalidOperationException>(() => _sut.SendMessage(message: _fakeMessage, topic: TestConstant.TOPIC_FAKE));
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public async Task SendMessageAsync_WhenProducerFails_ThrowsException()
    {
        string expectedErrorReason = "Partial message was received.";

        // Arrange
        SetupProduceAsyncThrowsException(errorReason: expectedErrorReason);

        // Act & Assert
        Exception ex = await Assert.ThrowsAsync<KafkaException>(async () => await _sut.SendMessageAsync(message: _fakeMessage));
        Assert.Equal(expectedErrorReason, ex.Message);
        VerifyProduceAsyncCalled(Times.Once());
    }

    [Theory]
    [InlineData(TestConstant.TOPIC_FAKE)]
    [InlineData(null)]
    public async Task SendMessageAsync_WhenProducerSucceeds_ReturnsDeliveryResult(string topic)
    {
        // Arrange

        var expectedDeliveryResult = DeliveryResultFactory<string, string>.CreateDeliveryResult(
            topic,
            message: _fakeMessage);

        SetupProduceAsyncSucceeds(expectedDeliveryResult);

        // Act
        var deliveryResult = await _sut.SendMessageAsync(message: _fakeMessage, topic, CancellationToken.None);

        // Assert
        Assert.Same(expectedDeliveryResult, deliveryResult);
        VerifyProduceAsyncCalled(Times.Once());
    }

    [Fact]
    public async Task SendMessageAsync_WhenCustomAvroSerializersProvided_ReturnsExpectedDeliveryResult()
    {
        // Arrange
        var eventFake = AvroRecordFake.Create();

        var message = KafkaMessageFactory.CreateKafkaMessage(value: eventFake, key: "key", headers: []);
        var expectedDeliveryResult = DeliveryResultFactory<string, AvroRecordFake>.CreateDeliveryResult(
            topic: TestConstant.TOPIC_FAKE,
            message);

        var mockSchemaRegistry = new Mock<ISchemaRegistryClient>();

        var mockProduce = new Mock<IProducer<string, AvroRecordFake>>();

        mockProduce
            .Setup(x => x.ProduceAsync(
               It.IsAny<string>(),
               It.IsAny<Message<string, AvroRecordFake>>(),
               It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDeliveryResult);

        _mockProducerFactory
          .Setup(f => f.CreateProducer(It.IsAny<IProducerConfiguration<string, AvroRecordFake>>()))
          .Returns(mockProduce.Object);

        var producerConfiguration = CreateProducerConfiguration<string, AvroRecordFake>(configure =>
        {
            configure.WithSerialization(configure =>
            {
                configure.SetKeySerializer(new AvroSerializer<string>(mockSchemaRegistry.Object).AsSyncOverAsync());
                configure.SetValueSerializer(new AvroSerializer<AvroRecordFake>(mockSchemaRegistry.Object).AsSyncOverAsync());
            });
        });

        var mockProducerConfiguration = new Mock<IProducerConfiguration<string, AvroRecordFake>>();
        mockProducerConfiguration
         .Setup(m => m.ProducerConfig)
         .Returns(producerConfiguration.ProducerConfig);

        mockProducerConfiguration
           .Setup(m => m.SerializersConfig)
           .Returns(producerConfiguration.SerializersConfig);



        var sut = new ProducerManager<string, AvroRecordFake>(
            logger: _mockLogger.Object,
            producerFactory: _mockProducerFactory.Object,
            producerConfiguration: mockProducerConfiguration.Object);

        // Act
        var deliveryResult = await sut.SendMessageAsync(message);

        // Assert
        Assert.Same(expectedDeliveryResult, deliveryResult);

        mockProduce.Verify(x => x.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<string, AvroRecordFake>>(),
                    It.IsAny<CancellationToken>()
                    ), Times.Once());
    }

    [Fact]
    public void SendMessage_WhenProducerSucceeds_InvokesDeliveryHandlerAndDisposes()
    {
        // Arrange
        SetupProduceSucceeds();

        bool deliveryHandlerCalled = false;
        void deliveryHandler(DeliveryReport<string, string> deliveryReport)
        {
            Assert.Equal(ErrorCode.NoError, deliveryReport.Error.Code);
            deliveryHandlerCalled = true;
        }

        // Act
        _sut.SendMessage(message: _fakeMessage, deliveryHandler: deliveryHandler);

        _sut.Dispose();

        // Assert
        Assert.True(deliveryHandlerCalled);
        VerifyProduceCalled(Times.Once());
        _mockProducer.Verify(p => p.Dispose(), Times.Once, "Producer should be disposed once.");
    }

    [Fact]
    public void SendMessage_WhenProducerSucceedsAndDisposed_InvokesDeliveryHandlerWithMessage()
    {
        // Arrange
        SetupProduceSucceeds();

        bool deliveryHandlerCalled = false;
        void deliveryHandler(Message<string, string> message, DeliveryReport<string, string> deliveryReport)
        {
            Assert.Equal(ErrorCode.NoError, deliveryReport.Error.Code);
            Assert.Same(_fakeMessage, message);
            deliveryHandlerCalled = true;
        }

        // Act
        _sut.SendMessage(message: _fakeMessage, deliveryHandler);
        _sut.Dispose();

        // Assert
        Assert.True(deliveryHandlerCalled);
        VerifyProduceCalled(Times.Once());
        _mockProducer.Verify(p => p.Dispose(), Times.Once);
    }

    [Fact]
    public void SendMessage_WhenProducerFails_ThrowsException()
    {
        // Arrange
        SetupProduceThrowsException();

        // Act & Assert
        Exception ex = Assert.Throws<ProduceException<string, string>>(() => _sut.SendMessage(message: _fakeMessage));
        Assert.Equal("Broker transport failure.", ex.Message);
        VerifyProduceCalled(Times.Once());
    }


    private ProducerManager<string, string> InitializeSut()
    {
        var producerManager = new ProducerManager<string, string>(
            logger: _mockLogger.Object,
            producerFactory: _mockProducerFactory.Object,
            producerConfiguration: _mockProducerConfiguration.Object);

        return producerManager;
    }
    private static ProducerConfiguration<TKey, TValue> CreateProducerConfiguration<TKey, TValue>(
        Action<ProducerConfigurationBuilder<TKey, TValue>> configure)
    {
        var producerConfigurationBuilder = CreateProducerConfigurationBuilder<TKey, TValue>();
        configure(producerConfigurationBuilder);
        return producerConfigurationBuilder.Build();
    }
    private static ProducerConfigurationBuilder<TKey, TValue> CreateProducerConfigurationBuilder<TKey, TValue>()
    {
        var producerBuilder = new ProducerConfigurationBuilder<TKey, TValue>(
            bootstrapServers: "localhost:9092",
            username: "username",
            password: "password");

        producerBuilder.Configure(configure =>
        {
            configure.SetName("producer-unique-name");
            configure.SetTopic(TestConstant.TOPIC_FAKE);
            configure.SetIdempotenceEnabled();
        });

        return producerBuilder;
    }
    private void SetupCreateProducer()
    {
        _mockProducerFactory
            .Setup(f => f.CreateProducer(It.IsAny<IProducerConfiguration<string, string>>()))
            .Returns(_mockProducer.Object);
    }
    private void SetupProducerConfiguration(IProducerConfiguration<string, string>? producerConfiguration = null)
    {

        producerConfiguration ??= CreateProducerConfigurationBuilder<string, string>().Build();

        _mockProducerConfiguration
            .Setup(m => m.ProducerConfig)
            .Returns(producerConfiguration.ProducerConfig);

        _mockProducerConfiguration
           .Setup(m => m.SerializersConfig)
           .Returns(producerConfiguration.SerializersConfig);
    }
    private void SetupProduceAsyncSucceeds(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducer.Setup(x => x.ProduceAsync(
            It.IsAny<string>(),
            It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
            It.IsAny<CancellationToken>()
            )).ReturnsAsync(deliveryResult);
    }
    private void SetupProduceAsyncThrowsException(string errorReason)
    {
        _mockProducer.Setup(x => x.ProduceAsync(
               It.IsAny<string>(),
               It.Is<Message<string, string>>(m => m.Key == _fakeMessage.Key && m.Value == _fakeMessage.Value),
               It.IsAny<CancellationToken>()
               )).ThrowsAsync(new KafkaException(new Error(ErrorCode.Local_Partial, errorReason)));
    }
    private void SetupProduceSucceeds()
    {
        _mockProducer.Setup(producer => producer.Produce(
            It.IsAny<string>(),
            It.Is<Message<string, string>>(m => m.Key == _fakeMessage.Key && m.Value == _fakeMessage.Value),
            It.IsAny<Action<DeliveryReport<string, string>>>()))
            .Callback<string, Message<string, string>, Action<DeliveryReport<string, string>>>((topic, message, handler) =>
            {
                handler(new DeliveryReport<string, string>() { Error = new Error(ErrorCode.NoError) });
            });
    }
    private void SetupProduceThrowsException()
    {
        var fakeError = new Error(ErrorCode.Local_Transport, reason: "Broker transport failure.");
        var fakeDeliveryResult = new DeliveryResult<string, string>
        {
            Topic = TestConstant.TOPIC_FAKE,
            Partition = new Partition(0),
            Offset = new Offset(5),
            Message = _fakeMessage,
            Status = PersistenceStatus.NotPersisted
        };

        _mockProducer.Setup(producer => producer.Produce(
             It.IsAny<string>(),
             It.Is<Message<string, string>>(m => m.Key == _fakeMessage.Key && m.Value == _fakeMessage.Value),
             It.IsAny<Action<DeliveryReport<string, string>>>()))
              .Throws(new ProduceException<string, string>(fakeError, fakeDeliveryResult, new InvalidOperationException("Inner exception message.")));
    }
    private void VerifyProduceAsyncCalled(Times times)
    {
        _mockProducer.Verify(x => x.ProduceAsync(
            It.IsAny<string>(),
            It.IsAny<Message<string, string>>(),
            It.IsAny<CancellationToken>()
            ), times);
    }
    private void VerifyProduceCalled(Times times)
    {
        _mockProducer.Verify(x => x.Produce(
            It.IsAny<string>(),
            It.IsAny<Message<string, string>>(),
            It.IsAny<Action<DeliveryReport<string, string>>>()
            ), times);
    }
}
