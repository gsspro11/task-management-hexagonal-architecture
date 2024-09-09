using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common.Exceptions;

/// <summary>
/// Represents errors that occur when a Kafka transaction is aborted.
/// This exception is thrown to indicate that a transaction within Kafka could not be completed successfully and was aborted.
/// </summary>
[ExcludeFromCodeCoverage]
public class KafkaTransactionAbortedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaTransactionAbortedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the reason for the transaction being aborted.</param>
    public KafkaTransactionAbortedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaTransactionAbortedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public KafkaTransactionAbortedException(
        string message,
        Exception innerException) : base(message, innerException)
    {
    }
}