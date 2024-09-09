using Poc.RabbitMQ.Configs;
using RabbitMQ.Client;
using System.Runtime;


namespace Poc.RabbitMQ.Factories;
internal interface IRabbitMQFactory<TEnumQueueName> : IDisposable
    where TEnumQueueName : struct, Enum
{
    TEnumQueueName GetQueueEnum();
    string GetQueueName();
    ushort GetMaxConcurrentMessages();
    string GetBrokerName();
    public PocRabbitMQConfig Config { get; }
    public PocRabbitMQQueueSettings Settings { get; }
    public IModel GetChannel();
    public RabbitMQMessage CreateRabbitMQMessage(string message, IBasicProperties properties = default!);

}
