namespace Poc.RabbitMQ.Results
{
    public class PocConsumeResult<T>
    {
        public string ConsumerTag { get; private set; }
        public ulong DeliveryTag { get; private set; }
        public bool Redelivered { get; private set; }
        public string Exchange { get; private set; }
        public string RoutingKey { get; private set; }
        public T Message { get; private set; }

        public PocConsumeResult(string consumerTag, ulong deliveryTag, bool redelivered,
            string exchange, string routingKey, T message)
        {
            ConsumerTag = consumerTag;
            DeliveryTag = deliveryTag;
            Redelivered = redelivered;
            Exchange = exchange;
            RoutingKey = routingKey;
            Message = message;

        }

    }
}
