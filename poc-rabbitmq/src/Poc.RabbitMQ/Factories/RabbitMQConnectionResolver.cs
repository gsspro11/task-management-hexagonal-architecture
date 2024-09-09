using RabbitMQ.Client;

namespace Poc.RabbitMQ.Factories
{
    internal delegate PocRabbitConnection RabbitMQConnectionResolver(
        string brokerName
    );
}
