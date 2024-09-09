using Poc.RabbitMQ.Results;
using RabbitMQ.Client;
using System.Text.Json;

namespace Poc.RabbitMQ.PubSub;

public interface IPocRabbitMQPubSub
{
    string GetQueueEnum();
}

public interface IPocRabbitMQPubSub<TMessage> : IPocRabbitMQPubSub, IDisposable
{
    
    Task PublishAsync(TMessage message, IBasicProperties props = null, bool mandatory = true,
        JsonSerializerOptions serializeOptions = null);
    
    Task BatchPublishAsync(IEnumerable<TMessage> messages, IBasicProperties props = null, bool mandatory = true,
        JsonSerializerOptions serializeOptions = null);
    
    Task ConsumeMessageAsync(
        Func<PocConsumeResult<TMessage>, Task> onMessageReceived,
        JsonSerializerOptions serializeOptions = null,
        CancellationToken cancellationToken = default);
    
    Task ConsumeMessageManualAckAsync(
        Func<PocConsumeResult<TMessage>, Task<bool>> onMessageReceived,
        JsonSerializerOptions serializeOptions = null,
        CancellationToken cancellationToken = default);
    
    Task ConsumeMessageManualAckFailedRedirectAsync(
        Func<PocConsumeResult<TMessage>, Task<bool>> onMessageReceived,
        JsonSerializerOptions serializeOptions = null, 
        int? retryCountConsumer = null,
        CancellationToken cancellationToken = default);
}
