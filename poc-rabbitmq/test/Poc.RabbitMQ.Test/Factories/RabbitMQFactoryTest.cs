using Poc.RabbitMQ.Configs;
using Poc.RabbitMQ.Factories;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace Poc.RabbitMQ.Test.Factories
{
    [ExcludeFromCodeCoverage]
    public static class RabbitMQFactoryTest
    {
        public static RabbitMQMessage CreateKafkaMessage(string body, IBasicProperties properties = default!)
        {
            return new RabbitMQMessage()
            {
                Body = System.Text.Encoding.UTF8.GetBytes(body),
                Properties = properties

            };
        }


        public static PocRabbitMQConfig CreatePocKafkaPubConfig(Action<PocRabbitMQConfig> configAction)
        {
            PocRabbitMQConfig config = new();
            configAction.Invoke(config);
            return config;
        }

    }


}
