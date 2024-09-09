namespace Poc.RabbitMQ.PubSub
{
    public class RabbitMQExchange
    {
        public string Exchange { get; set; }
        public string Type { get; set; }
        public bool Durable { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}
