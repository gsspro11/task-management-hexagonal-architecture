using Poc.Kafka.Configs;
using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Kafka.Test.Configs.Validators;

public class PocKafkaSchemaRegistryConfigTest
{
    [Fact]
    public void UseBrokerCredentials_SetsEnableUseBrokerCredentialsToTrue()
    {
        //Act
        var sut = new PocKafkaSchemaRegistryConfig();
        sut.UseBrokerCredentials();

        //Assert
        Assert.True(sut.EnableUseBrokerCredentials);
    }
}
