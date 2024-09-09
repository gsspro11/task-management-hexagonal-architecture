namespace Poc.RabbitMQ.PubSub
{
    public class RabbitMQQueue
    {
        public string Queue { get; set; }
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = true;
        public bool AutoDelete { get; set; } = true;
        public IDictionary<string, object> Arguments { get; set; } = null;
    }
}
