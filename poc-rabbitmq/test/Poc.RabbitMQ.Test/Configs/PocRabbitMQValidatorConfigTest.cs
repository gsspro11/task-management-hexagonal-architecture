using Poc.RabbitMQ.Configs;
using Poc.RabbitMQ.Test.Builders;

namespace Poc.RabbitMQ.Test.Configs;

public class PocRabbitMQValidatorConfigTest
{
    [Fact]
    public void ValidatorConfig_WhenconfigNull_ThrowsArgumentNullException()
    {
        var pubConfig = new PocRabbitMQConfigBuilder()
            .Build();

        Exception ex = Record.Exception(() => PocRabbitMQValidatorConfig.ValidateConfig(pubConfig));

        Assert.IsType<ArgumentNullException>(ex);
    }

    [Fact]
    public void ValidatorConfig_WhencHostNameNull_ThrowsArgumentNullException()
    {
        var pubConfig = new PocRabbitMQConfigBuilder()
            .WithSetServer(null, 5000)
            .Build();

        Exception ex = Record.Exception(() => PocRabbitMQValidatorConfig.ValidateConfig(pubConfig));

        Assert.IsType<ArgumentNullException>(ex);
    }


    [Fact]
    public void ValidatorConfig_WhenPortMinorZero_ThrowsArgumentNullException()
    {
        var pubConfig = new PocRabbitMQConfigBuilder()
            .WithSetServer("teste", 0)
            .Build();

        Exception ex = Record.Exception(() => PocRabbitMQValidatorConfig.ValidateConfig(pubConfig));

        Assert.IsType<ArgumentException>(ex);
    }


    [Fact]
    public void ValidatorConfig_WhenCredencialIsnull_ThrowsArgumentNullException()
    {
        var pubConfig = new PocRabbitMQConfigBuilder()
            .WithSetServer("teste", 500)
            .Build();

        Exception ex = Record.Exception(() => PocRabbitMQValidatorConfig.ValidateConfig(pubConfig));

        Assert.IsType<ArgumentException>(ex);
    }

}
