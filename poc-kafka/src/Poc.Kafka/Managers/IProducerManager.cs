using Confluent.Kafka;

namespace Poc.Kafka.Managers;

internal interface IProducerManager<TKey, TValue> : IDisposable
{
    void Flush(TimeSpan timeout);
    void InitTransactions(TimeSpan timeout);
    void BeginTransaction();
    void CommitTransaction();
    void AbortTransaction();
    Task<DeliveryResult<TKey, TValue>> SendMessageAsync(
       Message<TKey, TValue> message,
       string? topic = null,
       CancellationToken cancellationToken = default);

    void SendMessage(
      Message<TKey, TValue> message,
      string? topic = null,
      Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null);

    void SendMessage(
       Message<TKey, TValue> message,
       Action<Message<TKey, TValue>, DeliveryReport<TKey, TValue>> deliveryHandler,
       string? topic = null);
}
