namespace Poc.RabbitMQ.PubSub
{
    public delegate IPocRabbitMQPubSub PocRabbitMQPubSubResolver<TEnumQueueName>(
        TEnumQueueName queue
    ) where TEnumQueueName : struct, Enum;
}
