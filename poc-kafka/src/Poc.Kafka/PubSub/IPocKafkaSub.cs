using Poc.Kafka.Results;

namespace Poc.Kafka.PubSub;

/// <summary>
/// Define um contrato para a subscrição e recebimento de mensagens de um tópico Kafka, onde cada mensagem é composta por uma chave do tipo <typeparamref name="TKey"/> e um valor do tipo <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TKey">O tipo da chave usada para identificar mensagens dentro do tópico Kafka.</typeparam>
/// <typeparam name="TValue">O tipo do valor da mensagem recebida do tópico Kafka.</typeparam>
public interface IPocKafkaSub<TKey, TValue> : IAsyncDisposable
{
    /// <summary>
    /// Initiates an asynchronous operation to continuously consume Kafka messages. This method runs in the background, 
    /// invoking a specified callback function for each message received. It provides an efficient way to handle incoming messages 
    /// in real-time as they arrive from the Kafka topic.
    /// </summary>
    /// <param name="onMessageReceived">The callback function that is called for each message received from Kafka. 
    /// This function should contain the logic for processing the message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the consume operation, 
    /// allowing for a graceful shutdown of the message consumption process.</param>
    /// <returns>A task representing the ongoing consumption operation. This task runs indefinitely until it is either cancelled via the cancellation token or an unhandled exception occurs.</returns>
    Task ConsumeAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken);
}
