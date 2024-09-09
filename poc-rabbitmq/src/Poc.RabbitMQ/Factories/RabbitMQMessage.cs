using RabbitMQ.Client;

namespace Poc.RabbitMQ.Factories
{
    public class RabbitMQMessage
    {
        public IBasicProperties Properties { get; set; }
        public byte[] Body { get; set; }
    }
}
