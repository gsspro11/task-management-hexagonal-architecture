using Poc.Kafka.Common.Constants;
using Poc.Kafka.Results;

namespace Poc.Kafka.Common.Extensions;

/// <summary>
/// Provides extension methods for handling retry logic of Kafka messages within the PocConsumeResult context.
/// </summary>
public static class PocConsumeResultExtension
{
    /// <summary>
    /// Marks the specified message for retry, indicating a processing failure and the intention to reprocess it.
    /// This method is intended to be called when a message consumed from a Kafka retry topic requires further attempts
    /// due to temporary issues. It is only applicable if the 'EnableRetryTopicSubscription' property is set to true,
    /// which signifies that the retry mechanism is managed within the same consuming process. Invoking this method
    /// signals the intent to reattempt processing the message by setting the 'ShouldRetry' property to true, thereby
    /// scheduling it for another processing cycle at the appropriate point in the message flow. It provides a controlled
    /// approach to handle transient failures, ensuring messages are given adequate opportunities for processing before
    /// being moved to a Dead Letter Queue or otherwise handled as failed.
    /// </summary>
    /// <typeparam name="TKey">The type of the key in the message, indicating the unique identifier for the message.</typeparam>
    /// <typeparam name="TValue">The type of the value in the message, representing the message's payload.</typeparam>
    /// <param name="pocConsumeResult">The consume result object that encapsulates the message to be retried, including its key, value, and metadata.</param>
    public static void TryAgain<TKey, TValue>(this PocConsumeResult<TKey, TValue> pocConsumeResult) =>
        pocConsumeResult.MarkForRetry();

    /// <summary>
    /// Marks the specified consume result to skip retry logic and instead send the message directly to the Dead Letter Queue (DLQ).
    /// This extension method is intended for use with messages that have encountered a processing error deemed unrecoverable,
    /// allowing for immediate redirection to the DLQ without further retry attempts. It is particularly useful in scenarios
    /// where continued retrying is known to be futile or potentially harmful, providing a mechanism for graceful error handling
    /// and message triage.
    /// </summary>
    /// <typeparam name="TKey">The type of the key of the message.</typeparam>
    /// <typeparam name="TValue">The type of the value of the message.</typeparam>
    /// <param name="pocConsumeResult">The consume result instance to be marked for direct sending to the DLQ.</param>
    public static void SkipRetryAndSendToDeadLetter<TKey, TValue>(this PocConsumeResult<TKey, TValue> pocConsumeResult) =>
        pocConsumeResult.MarkForDirectDeadLetter();

    /// <summary>
    /// Determines whether the number of retry attempts for a given message has exceeded the specified retry limit.
    /// This method checks the retry count against the retry limit defined in the message's metadata.
    /// It is useful for deciding whether to stop retrying the message processing and possibly move it to a dead letter queue.
    /// </summary>
    /// <typeparam name="TKey">The type of the key in the message, indicating the unique identifier for the message.</typeparam>
    /// <typeparam name="TValue">The type of the value in the message, representing the message's payload.</typeparam>
    /// <param name="pocConsumeResult">The consume result object that encapsulates the message to be retried, including its key, value, and metadata.</param>
    /// <returns>True if the retry count has reached or exceeded the limit; otherwise, false.</returns>
    public static bool IsRetryLimitExceeded<TKey, TValue>(this PocConsumeResult<TKey, TValue> pocConsumeResult)
    {
        int retryCount = pocConsumeResult.Message.Headers.GetHeaderAs<int>(ConsumerConstant.HEADER_NAME_RETRY_COUNT);
        return retryCount >= pocConsumeResult.RetryLimit;
    }
}
