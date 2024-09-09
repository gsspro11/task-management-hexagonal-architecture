using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Configs.Validators;

public class PocKafkaSchemaRegisterConfigValidatorTest
{
    private readonly PocKafkaSchemaRegistryConfig _schemaRegistryConfig;
    public PocKafkaSchemaRegisterConfigValidatorTest() =>
        _schemaRegistryConfig = CreateSchemaRegistryConfig();

    [Fact]
    public void Validate_WhenValidConfigs_ValidatesSuccess()
    {
        // Act
        var exceptionRecorded = Record.Exception(() => PocKafkaSchemaRegisterConfigValidator.Validate(_schemaRegistryConfig));

        // Assert
        Assert.Null(exceptionRecorded);
    }


    [Fact]
    public void Validate_WhenConfigIsNull_ThrowsArgumentNullException() =>
        Assert.Throws<ArgumentNullException>(() => PocKafkaSchemaRegisterConfigValidator.Validate(null!));

    [Fact]
    public void Validate_WhenUrlIsNull_ThrowsArgumentNullException()
    {
        //Arrange
        string expectedErrorMessage = "Value cannot be null. (Parameter 'schemaRegistryConfig.Url')";

        _schemaRegistryConfig.SetUrl(null!);

        //Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => PocKafkaSchemaRegisterConfigValidator.Validate(_schemaRegistryConfig));
        Assert.Equal(expectedErrorMessage, ex.Message);
    }

    [Theory]
    [InlineData("XXXXXXXX", null!)]
    [InlineData(null!, "XXXXXXXX")]
    public void Validate_GivenInvalidCredentials_ThrowsArgumentException(string username, string password)
    {
        //Arrange
        string expectedErrorMessage = "Username and Password are required.";

        _schemaRegistryConfig.SetCredentials(username, password);

        //Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => PocKafkaSchemaRegisterConfigValidator.Validate(_schemaRegistryConfig));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }



    private static PocKafkaSchemaRegistryConfig CreateSchemaRegistryConfig()
    {
        var consumerConfig = new PocKafkaSchemaRegistryConfig();

        consumerConfig.SetUrl("localhost:8081");
        consumerConfig.SetCredentials(username: "XXXXXXXX", password: "XXXXXXXX");
        return consumerConfig;
    }
}
