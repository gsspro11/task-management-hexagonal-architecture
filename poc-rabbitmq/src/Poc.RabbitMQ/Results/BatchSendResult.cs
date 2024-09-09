using Poc.RabbitMQ.Factories;

namespace Poc.RabbitMQ.Results;
public class BatchSendResult
{
    public List<(RabbitMQMessage message, bool)> Successes { get; set; } = new();
    public List<(RabbitMQMessage message, Exception exception)> Failures { get; set; } = new();
}
