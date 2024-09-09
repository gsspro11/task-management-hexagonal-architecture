using Poc.Kafka.Common.Exceptions;
using Poc.Kafka.Factories;
using Poc.Kafka.Test.CommonTests;
using Poc.Kafka.Test.CommonTests.Factories;
using Confluent.Kafka;
using Moq;

namespace Poc.Kafka.Test.PubSub;

public partial class PocKafkaPubTest
{
    [Fact]
    public async Task SendBatchAsync_WhenAllMessagesSuccess_ReturnsBatchSendResultWithoutFailures()
    {
        // Arrange
        var (successMessages, _) = CreateBatchMessages(successCount: 5);
        SetupMultipleSendMessageAsyncSucceeds(successMessages);

        var messages = ConvertToKafkaMessages(successMessages);

        // Act
        var batchResult = await _sut.SendBatchAsync(messages);

        // Assert
        Assert.Equal(successMessages.Count, batchResult.Successes.Count);
        Assert.Empty(batchResult.Failures);
        Assert.False(batchResult.HasFailures);
        VerifySendMessageAsyncWithTopicCalled(Times.Exactly(successMessages.Count));
    }

    [Fact]
    public async Task SendBatchAsync_WhenSomeMessagesSucceedAndOthersFail_ReturnsBatchSendResultWithFailures()
    {
        //Arrange
        Guid expectedBatchId = Guid.NewGuid();
        var expectedErrorReason = GetExpectedErrorReasonDuringMessageDelivery(expectedBatchId);

        var (successMessages, failuresMessages) = CreateBatchMessages(successCount: 5, failureCount: 5);
        SetupMultipleSendMessageAsyncSucceeds(successMessages);
        SetupMultipleSendMessageAsyncThrowsException(messages: failuresMessages, errorReason: expectedErrorReason);

        var allMessages = successMessages.Concat(failuresMessages).ToList();

        var messages = ConvertToKafkaMessages(allMessages);
        // Act       
        var batchResult = await _sut.SendBatchAsync(
            messages,
            batchId: expectedBatchId,
            cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(successMessages.Count, batchResult.Successes.Count);
        Assert.True(batchResult.HasFailures);
        Assert.Equal(failuresMessages.Count, batchResult.Failures.Count);
        Assert.Contains(batchResult.Failures, f => f.Error == expectedErrorReason);
        VerifySendMessageAsyncWithTopicCalled(Times.Exactly(messages.Count));
    }

    [Fact]
    public void SendBatch_WhenAllMessagesProduceSuccess_ReturnsBatchSendResultWithoutFailures()
    {
        // Arrange
        var (successMessages, _) = CreateBatchMessages(successCount: 5);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackSuccess(successMessages);

        var messages = ConvertToKafkaMessages(successMessages);

        // Act
        var batchResult = _sut.SendBatch(messages);

        // Assert
        Assert.Equal(successMessages.Count, batchResult.Successes.Count);
        Assert.Empty(batchResult.Failures);
        Assert.False(batchResult.HasFailures);
        VerifySendMessageWithMessageAndDeliveryReportCallbackCalled(Times.Exactly(successMessages.Count));
        VerifyFlushCalled();
    }

    [Fact]
    public void SendBatch_WhenSomeMessagesSucceedAndOthersFail_ReturnsBatchSendResultWithFailures()
    {
        //Arrange
        Guid expectedBatchId = Guid.NewGuid();
        var expectedErrorReason = GetExpectedErrorReasonDuringMessageDelivery(expectedBatchId);

        var (successMessages, failuresMessages) = CreateBatchMessages(successCount: 5, failureCount: 5);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackSuccess(successMessages);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackException(
            messages: failuresMessages,
            exception: new KafkaException(new Error(ErrorCode.Local_Partial, expectedErrorReason)));

        var allMessages = successMessages.Concat(failuresMessages).ToList();

        var messages = ConvertToKafkaMessages(allMessages);

        // Act
        var batchResult = _sut.SendBatch(messages, batchId: expectedBatchId);

        // Assert
        Assert.Equal(successMessages.Count, batchResult.Successes.Count);
        Assert.True(batchResult.HasFailures);
        Assert.Equal(failuresMessages.Count, batchResult.Failures.Count);
        Assert.Contains(batchResult.Failures, f => f.Error == expectedErrorReason);
        VerifySendMessageWithMessageAndDeliveryReportCallbackCalled(Times.Exactly(allMessages.Count));
        VerifyFlushCalled();
    }


    [Fact]
    public void SendBatch_WhenSomeMessagesSucceedAndOthersError_ReturnsBatchSendResultWithFailures()
    {
        //Arrange
        var (successMessages, failuresMessages) = CreateBatchMessages(successCount: 5, failureCount: 5);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackSuccess(successMessages);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackError(messages: failuresMessages);

        var allMessages = successMessages.Concat(failuresMessages).ToList();

        var messages = ConvertToKafkaMessages(allMessages);

        // Act
        var batchResult = _sut.SendBatch(messages);

        // Assert
        Assert.Equal(successMessages.Count, batchResult.Successes.Count);
        Assert.True(batchResult.HasFailures);
        Assert.Equal(failuresMessages.Count, batchResult.Failures.Count);
        Assert.Contains(batchResult.Failures, f => f.Error == TestConstant.EXPECTED_ERROR_REASON_PARTIAL_MESSAGE);
        VerifySendMessageWithMessageAndDeliveryReportCallbackCalled(Times.Exactly(allMessages.Count));
        VerifyFlushCalled();
    }

    [Fact]
    public void SendBatch_WhenMessagesAreUndelivered_MarksThemAsFailed()
    {
        // Arrange
        var (deliveredMessages, errorsMessages) = CreateBatchMessages(successCount: 5, failureCount: 5);
        SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackSuccess(deliveredMessages);
        SetupMultipleSendMessageWithoutDeliveryConfirmation(errorsMessages);

        var allMessages = deliveredMessages.Concat(errorsMessages).ToList();

        var messages = ConvertToKafkaMessages(allMessages);

        // Act
        var batchResult = _sut.SendBatch(messages);

        // Assert
        Assert.Equal(deliveredMessages.Count, batchResult.Successes.Count);
        Assert.True(batchResult.HasFailures);
        Assert.Equal(errorsMessages.Count, batchResult.Failures.Count);
        Assert.Contains(batchResult.Failures, f => f.Error == "Delivery timed out or status uncertain.");
        VerifySendMessageWithMessageAndDeliveryReportCallbackCalled(Times.Exactly(allMessages.Count));
        VerifyFlushCalled();
    }

    [Fact]
    public void SendBatchAtomic_WhenAAllMessagesSucceed_TransactionCommittedAndReturnsTrue()
    {
        // Arrange
        var (successMessages, _) = CreateBatchMessages(successCount: 5);
        SetupMultipleSendMessageWithDeliveryReportCallbackSuccess(successMessages);

        var message = ConvertToKafkaMessages(successMessages);

        // Act
        bool batchResult = _sut.SendBatchAtomic(message);

        // Assert
        Assert.True(batchResult);

        _mockProducerManager.Verify(x => x.InitTransactions(It.IsAny<TimeSpan>()), Times.Once());
        _mockProducerManager.Verify(x => x.BeginTransaction(), Times.Once());
        _mockProducerManager.Verify(x => x.CommitTransaction(), Times.Once());
        _mockProducerManager.Verify(x => x.AbortTransaction(), Times.Never());

        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Exactly(successMessages.Count));
    }

    [Fact]
    public void SendBatchAtomic_WhenAnyMessageFails_TransactionAbortedAndReturnsFalse()
    {
        // Arrange
        var (successMessages, failuresMessages) = CreateBatchMessages(successCount: 5, failureCount: 1);
        SetupMultipleSendMessageWithDeliveryReportCallbackSuccess(successMessages);
        SetupMultipleSendMessageWithDeliveryReportCallbackException(
            messages: failuresMessages,
            exception: new KafkaTransactionAbortedException("One message sending failure transaction aborted."));

        var messages = successMessages.Concat(failuresMessages).ToList();
        var message = ConvertToKafkaMessages(messages);

        int expectedSendMessageInvocations = successMessages.Count + failuresMessages.Count;

        // Act
        bool batchResult = _sut.SendBatchAtomic(message);

        // Assert
        Assert.False(batchResult);

        _mockProducerManager.Verify(x => x.InitTransactions(It.IsAny<TimeSpan>()), Times.Once());
        _mockProducerManager.Verify(x => x.BeginTransaction(), Times.Once());
        _mockProducerManager.Verify(x => x.CommitTransaction(), Times.Never());
        _mockProducerManager.Verify(x => x.AbortTransaction(), Times.Once());

        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Exactly(expectedSendMessageInvocations));
    }

    [Fact]
    public void SendBatchAtomic_WhenAnyMessageErrors_TransactionAbortedAndReturnsFalse()
    {
        // Arrange
        var (successMessages, failuresMessages) = CreateBatchMessages(successCount: 5, failureCount: 1);
        SetupMultipleSendMessageWithDeliveryReportCallbackSuccess(successMessages);
        SetupMultipleSetupSendMessageWithDeliveryReportCallbackcError(messages: failuresMessages);

        var messages = successMessages.Concat(failuresMessages).ToList();
        var message = ConvertToKafkaMessages(messages);

        int expectedSendMessageInvocations = successMessages.Count + failuresMessages.Count;

        // Act
        bool batchResult = _sut.SendBatchAtomic(message);

        // Assert
        Assert.False(batchResult);

        _mockProducerManager.Verify(x => x.InitTransactions(It.IsAny<TimeSpan>()), Times.Once());
        _mockProducerManager.Verify(x => x.BeginTransaction(), Times.Once());
        _mockProducerManager.Verify(x => x.CommitTransaction(), Times.Never());
        _mockProducerManager.Verify(x => x.AbortTransaction(), Times.Once());

        VerifySendMessageWithDeliveryReportCallbackCalled(Times.Exactly(expectedSendMessageInvocations));
    }

    private static (List<(string Key, string Value)> SuccessMessages, List<(string Key, string Value)> FailureMessages) CreateBatchMessages(
     int successCount = 0, int failureCount = 0)
    {
        var successMessages = new List<(string Key, string Value)>();
        var failureMessages = new List<(string Key, string Value)>();

        for (int i = 1; i <= successCount; i++)
        {
            successMessages.Add(($"Key-{i}", $"Message {i}"));
        }

        for (int i = 1; i <= failureCount; i++)
        {
            failureMessages.Add(($"Key-F{i}", $"Message F{i}"));
        }

        return (successMessages, failureMessages);
    }
    private static List<Message<string, string>> ConvertToKafkaMessages(List<(string Key, string Value)> messages) =>
          messages.Select(m => KafkaMessageFactory.CreateKafkaMessage(value: m.Value, key: m.Key)).ToList();
    private static string GetExpectedErrorReasonDuringMessageDelivery(Guid batchId) =>
            $"An error occurred during message delivery for batch {batchId}.";



    private void SetupMultipleSendMessageAsyncSucceeds(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageAsyncSuccess(deliveryResult);
        }
    }

    private void SetupMultipleSendMessageAsyncThrowsException(List<(string Key, string Value)> messages, string errorReason)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageAsyncThrowsException(deliveryResult, errorReason);
        }
    }

    private void SetupSendMessageAsyncThrowsException(DeliveryResult<string, string> deliveryResult, string errorReason)
    {
        _mockProducerManager
            .Setup(x => x.SendMessageAsync(
               It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
               It.IsAny<string>(),
               It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KafkaException(new Error(ErrorCode.Local_Partial, errorReason)));
    }

    private void SetupMultipleSendMessageWithDeliveryReportCallbackException(List<(string Key, string Value)> messages, Exception exception)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithDeliveryReportCallbackThrowsException(deliveryResult, exception);
        }
    }

    private void SetupSendMessageWithDeliveryReportCallbackThrowsException(DeliveryResult<string, string> deliveryResult, Exception exception)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<string>(),
                It.IsAny<Action<DeliveryReport<string, string>>>()))
            .Throws(exception);
    }

    private void SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackException(List<(string Key, string Value)> messages, Exception exception)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithMessageAndDeliveryReportCallbackThrowsException(deliveryResult, exception);
        }
    }

    private void SetupSendMessageWithMessageAndDeliveryReportCallbackThrowsException(DeliveryResult<string, string> deliveryResult, Exception exception)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<Action<Message<string, string>, DeliveryReport<string, string>>>(),
                It.IsAny<string>()))
            .Throws(exception);
    }

    private void SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackSuccess(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithMessageAndDeliveryReportCallbackSuccess(deliveryResult);
        }
    }
    private void SetupSendMessageWithMessageAndDeliveryReportCallbackSuccess(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
                It.IsAny<Action<Message<string, string>, DeliveryReport<string, string>>>(),
                It.IsAny<string>()))
            .Callback<Message<string, string>, Action<Message<string, string>, DeliveryReport<string, string>>, string>((message, handler, topic) =>
            {
                var deliveryReport = CreateDeliveryReportSuccess(deliveryResult, message);
                handler(message, deliveryReport);
            });
    }

    private void SetupMultipleSendMessageWithMessageAndDeliveryReportCallbackError(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithMessageAndDeliveryReportCallbackError(deliveryResult);
        }
    }

    private void SetupSendMessageWithMessageAndDeliveryReportCallbackError(DeliveryResult<string, string> deliveryResult)
    {
        _mockProducerManager
           .Setup(producer => producer.SendMessage(
               It.Is<Message<string, string>>(m => m.Key == deliveryResult.Key && m.Value == deliveryResult.Value),
               It.IsAny<Action<Message<string, string>, DeliveryReport<string, string>>>(),
               It.IsAny<string>()))
           .Callback<Message<string, string>, Action<Message<string, string>, DeliveryReport<string, string>>, string>((message, handler, topic) =>
           {
               var errorReport = CreateDeliveryReportError(deliveryResult, message);
               handler(message, errorReport);
           });
    }

    private void SetupMultipleSendMessageWithoutDeliveryConfirmation(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            SetupSendMessageWithoutDeliveryConfirmation(key, value);
        }
    }

    private void SetupSendMessageWithoutDeliveryConfirmation(string key, string value)
    {
        _mockProducerManager
            .Setup(producer => producer.SendMessage(
                It.Is<Message<string, string>>(m => m.Key == key && m.Value == value),
                It.IsAny<Action<Message<string, string>, DeliveryReport<string, string>>>(),
                It.IsAny<string>()));
    }
    private void SetupMultipleSendMessageWithDeliveryReportCallbackSuccess(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithDeliveryReportCallbackcSuccess(deliveryResult);
        }
    }
    private void SetupMultipleSetupSendMessageWithDeliveryReportCallbackcError(List<(string Key, string Value)> messages)
    {
        foreach (var (key, value) in messages)
        {
            var deliveryResult = CreateDeliveryResult(key, value);
            SetupSendMessageWithDeliveryReportCallbackcError(deliveryResult);
        }
    }

    private static DeliveryResult<string, string> CreateDeliveryResult(string key, string value)
    {
        var message = KafkaMessageFactory.CreateKafkaMessage(value, key, headers: []);
        return DeliveryResultFactory<string, string>.CreateDeliveryResult(topic: TestConstant.TOPIC_FAKE, message: message);
    }

    private void VerifyFlushCalled() =>
        _mockProducerManager.Verify(x => x.Flush(It.IsAny<TimeSpan>()), Times.Once());

    private void VerifySendMessageWithMessageAndDeliveryReportCallbackCalled(Times times)
    {
        _mockProducerManager
            .Verify(x => x.SendMessage(
                It.IsAny<Message<string, string>>(),
                It.IsAny<Action<Message<string, string>, DeliveryReport<string, string>>>(),
                It.IsAny<string>()), times);
    }


}
