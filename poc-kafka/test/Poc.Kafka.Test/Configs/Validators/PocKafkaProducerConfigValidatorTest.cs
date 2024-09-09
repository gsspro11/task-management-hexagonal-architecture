using Poc.Kafka.Common.Constants;
using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Configs.Validators;

public class PocKafkaProducerConfigValidatorTest
{
    private readonly PocKafkaProducerConfig _producerConfig;
    public PocKafkaProducerConfigValidatorTest() =>
        _producerConfig = CreateProducerConfig();

    [Fact]
    internal void Validate_WhenValidConfigs_ValidatesSuccessfully()
    {
        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WhenNameIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'producerConfig.Name')";

        _producerConfig.SetName(null!);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData("XXXXXXXX", null!)]
    [InlineData(null!, "XXXXXXXX")]
    public void Validate_GivenInvalidCredentials_ThrowsArgumentException(string username, string password)
    {
        string expectedErrorMessage = "Username and Password are required.";

        _producerConfig.SetCredentials(username, password);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenBootstrapServersIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'producerConfig.BootstrapServers')";

        _producerConfig.SetBootstrapServers(null!);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenTopicIsNull_ThrowsArgumentNullException()
    {
        string expectedErrorMessage = "Value cannot be null. (Parameter 'producerConfig.Topic')";

        _producerConfig.SetTopic(null!);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentNullException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ProducerConstant.MIN_MESSAGE_SEND_RETRIES - 1)]
    [InlineData(ProducerConstant.MAX_MESSAGE_SEND_RETRIES + 1)]
    public void Validate_GivenMessageSendMaxRetriesIsOutOfRange_ThrowsArgumentException(int messageSendMaxRetries)
    {
        string expectedErrorMessage = $"The minimum value allowed for the MessageSendMaxRetries property is " +
            $"{ProducerConstant.MIN_MESSAGE_SEND_RETRIES} and the maximum is {ProducerConstant.MAX_MESSAGE_SEND_RETRIES}. Current value: {messageSendMaxRetries}.";

        _producerConfig.SetMessageSendMaxRetries(messageSendMaxRetries);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(ProducerConstant.MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE - 1)]
    [InlineData(ProducerConstant.MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE + 1)]
    public void Validate_GivenIdempotenceEnabledAndMaxInFlightIsOutOfRange_ThrowsArgumentException(int maxInFlight)
    {
        string expectedErrorMessage = $"When EnableIdempotence is enabled, the minimum value allowed for the MaxInFlight " +
            $"property is {ProducerConstant.MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE} and the maximum is {ProducerConstant.MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE}. Current value: {maxInFlight}.";

        _producerConfig.SetIdempotenceEnabled();
        _producerConfig.SetMaxInFlight(maxInFlight);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Fact]
    public void Validate_WhenIdempotenceEnabledAndMessageSendMaxRetriesExceedsLimit_ThrowsArgumentException()
    {
        int messageSendMaxRetries = 4;

        string expectedErrorMessage = $"When EnableIdempotence is enabled, the maximum value allowed for the MessageSendMaxRetries " +
            $"property is {ProducerConstant.MAX_MESSAGE_SEND_RETRIES_WITH_ENABLE_IDEMPOTENCE}. Current value: {messageSendMaxRetries}.";

        _producerConfig.SetIdempotenceEnabled();
        _producerConfig.SetMessageSendMaxRetries(messageSendMaxRetries);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData(Acks.None)]
    [InlineData(Acks.Leader)]
    public void Validate_GivenIdempotenceEnabledAndAcksIsNotAll_ThrowsArgumentException(Acks acks)
    {
        string expectedErrorMessage = $"When EnableIdempotence is enabled, the value for the Acks property is required Acks.All. Current value: {acks}.";

        _producerConfig.SetIdempotenceEnabled();
        _producerConfig.SetAcks(acks);

        Exception ex = Record.Exception(() => PocKafkaProducerConfigValidator.Validate(_producerConfig));

        Assert.IsType<ArgumentException>(ex);
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    private static PocKafkaProducerConfig CreateProducerConfig()
    {
        var producerConfig = new PocKafkaProducerConfig();
        producerConfig.SetName("unique-producer-name");
        producerConfig.SetBootstrapServers("localhost:9092");
        producerConfig.SetCredentials(username: "XXXXXXXX", password: "XXXXXXXX");
        producerConfig.SetTopic("topic");
        producerConfig.SetIdempotenceEnabled();
        return producerConfig;
    }
}
