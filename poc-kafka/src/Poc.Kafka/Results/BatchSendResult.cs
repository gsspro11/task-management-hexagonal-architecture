using Confluent.Kafka;

namespace Poc.Kafka.Results;

/// <summary>
/// Represents the result of a batch send operation in Kafka. This class encapsulates the outcomes of sending a batch of messages, 
/// categorizing them into successes and failures.
/// </summary>
/// <typeparam name="TKey">The type of the key used in the Kafka messages.</typeparam>
/// <typeparam name="TValue">The type of the value used in the Kafka messages.</typeparam>
/// <remarks>
/// <para>
/// The 'Successes' property holds a list of successfully delivered messages along with their delivery reports, 
/// while the 'Failures' property contains a list of messages that failed to send, each paired with an error description.
/// </para>
/// <para>
/// The 'HasFailures' property is a convenience flag indicating whether any messages in the batch failed to be sent.
/// </para>
/// </remarks>
public sealed class BatchSendResult<TKey, TValue>
{
    /// <summary>
    /// Gets or sets a list of successful delivery results. Each <see cref="DeliveryResult{TKey, TValue}"/> 
    /// in the list represents a message that was successfully sent.
    /// </summary>
    public List<DeliveryResult<TKey, TValue>> Successes { get; set; } = new();

    /// <summary>
    /// Gets or sets a list of tuples representing the failures in the batch. Each tuple contains the failed 
    /// <see cref="Message{TKey, TValue}"/> and a string describing the error.
    /// </summary>
    public List<(Message<TKey, TValue> Message, string Error)> Failures { get; set; } = new();

    /// <summary>
    /// Indicates whether there were any failures in the batch send operation.
    /// </summary>
    public bool HasFailures => Failures.Any();
}
