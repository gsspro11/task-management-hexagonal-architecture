using Poc.RabbitMQ.Factories;
using Microsoft.Extensions.Logging;

namespace Poc.RabbitMQ.PubSub;

internal class PocRabbitMQPubSub<TEnumQueueName, TMessage> : PocRabbitMQPubSubBase<TEnumQueueName, TMessage>
    where TEnumQueueName : struct, Enum
{
    public PocRabbitMQPubSub(
        IRabbitMQFactory<TEnumQueueName> rabbitMQFactory, 
        ILogger<IPocRabbitMQPubSub> logger
    )
        : base(rabbitMQFactory, logger) { }
}
