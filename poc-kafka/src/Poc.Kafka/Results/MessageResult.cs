using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Results;

[ExcludeFromCodeCoverage]
internal sealed class MessageResult<TKey, TValue>
{
    internal MessageResult(Message<TKey, TValue> message)
    {
        Message = message;
        IsDelivered = false;
        IsError = false;
    }

    internal Message<TKey, TValue> Message { get; private set; }
    internal bool IsDelivered { get; private set; }
    internal bool IsError { get; private set; }
    internal string? ErrorMessage { get; private set; }
    internal DeliveryResult<TKey, TValue>? DeliveryResult { get; private set; }
    internal void SetErrorMessage(string errorMessage)
    {
        IsDelivered = false;
        IsError = true;
        ErrorMessage = errorMessage;
    }
    internal void SetDeliveryResult(DeliveryResult<TKey, TValue> deliveryResult)
    {
        IsDelivered = true;
        IsError = false;
        DeliveryResult = deliveryResult;
    }
}
