namespace Poc.RabbitMQ.PubSub
{
    public class RabbitMQQueueBind
    {
        public string Queue { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
