using Confluent.Kafka;

namespace Poc.Kafka.Results;

/// <summary>
/// Extends the <see cref="ConsumeResult{TKey, TValue}"/>  class from the Confluent.Kafka library to include retry management capabilities for message processing.
/// This class introduces additional properties and methods to control whether a consumed message should be retried in the event of a processing failure
/// and to specify a limit on the number of retry attempts. It is designed to enhance message processing systems by providing 
/// more granular control over message retry logic, making it easier to manage temporary failures and ensure reliable message processing.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class PocConsumeResult<TKey, TValue> : ConsumeResult<TKey, TValue>
{
    /// <summary>
    /// Indicates whether the message should be retried after a processing failure.
    /// Initially set to false, it can be marked for retry by calling the MarkForRetry method.
    /// </summary>
    public bool ShouldRetry { get; private set; } = false;

    /// <summary>
    /// Specifies the maximum number of attempts to retry message processing.
    /// This limit is defined by the initial consumer configuration and cannot be altered after instantiation.
    /// </summary>
    public int RetryLimit { get; init; }

    /// <summary>
    /// Indicates whether the message should be sent to the dead letter queue after a processing failure.
    /// </summary>
    public bool SkipRetryAndSendToDeadLetter { get; private set; } = false;

    /// <summary>
    /// Initializes a new instance of the PocConsumeResult class using an existing ConsumeResult instance.
    /// This constructor copies all relevant message consumption details from the provided ConsumeResult,
    /// including message, topic, partition, offset, and whether the end of the partition has been reached.
    /// </summary>
    /// <param name="consumeResult">The result of a message consumption operation, containing key and value among other details.</param>
    private PocConsumeResult(ConsumeResult<TKey, TValue> consumeResult)
    {
        Message = consumeResult.Message;
        Topic = consumeResult.Topic;
        Partition = consumeResult.Partition;
        Offset = consumeResult.Offset;
        TopicPartitionOffset = consumeResult.TopicPartitionOffset;
        IsPartitionEOF = consumeResult.IsPartitionEOF;
    }

    /// <summary>
    /// Marks the current message for retry, indicating a processing failure has occurred and the message should be reprocessed.
    /// This method sets the ShouldRetry property to true.
    /// </summary>
    internal void MarkForRetry() =>
        ShouldRetry = true;

    /// <summary>
    /// Marks the current message for sending to the dead letter queue, indicating a processing failure has occurred and the message should be discarded.
    /// </summary>
    internal void MarkForDirectDeadLetter() =>
      SkipRetryAndSendToDeadLetter = true;


    internal static PocConsumeResult<TKey, TValue> Create(
        ConsumeResult<TKey, TValue> consumeResult, int retryLimit) => new(consumeResult)
        {
            RetryLimit = retryLimit
        };
}
