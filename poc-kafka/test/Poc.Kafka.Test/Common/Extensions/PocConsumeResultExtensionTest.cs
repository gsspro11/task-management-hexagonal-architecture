using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Factories;
using Poc.Kafka.Results;
using Confluent.Kafka;

namespace Poc.Kafka.Test.Common.Extensions;

public class PocConsumeResultExtensionTest
{
    private readonly PocConsumeResult<string, string> _sut;
    public PocConsumeResultExtensionTest() =>
        _sut = InitializeSut();

    [Fact]
    public void TryAgain_WhenInvoked_SetsShouldRetryToTrue()
    {
        _sut.TryAgain();

        Assert.True(_sut.ShouldRetry);
    }

    [Fact]
    public void SkipRetryAndSendToDeadLetter_WhenInvoked_SetsSkipRetryAndSendToDeadLetterToTrue()
    {
        _sut.SkipRetryAndSendToDeadLetter();

        Assert.True(_sut.SkipRetryAndSendToDeadLetter);
    }

    [Fact]
    public void IsRetryLimitExceeded_WhenRetryLimitIsNotExceeded_ReturnsFalse() =>
        Assert.False(_sut.IsRetryLimitExceeded());

    [Fact]
    public void IsRetryLimitExceeded_WhenRetryCountExceedsRetryLimit_ReturnsTrue()
    {
        int retryLimitExceeded = _sut.RetryLimit + 1;

        byte[] retryLimitExceededAsBytes = BitConverter.GetBytes(retryLimitExceeded);

        _sut.Message.Headers.AddOrUpdate(ConsumerConstant.HEADER_NAME_RETRY_COUNT, retryLimitExceededAsBytes);

        Assert.True(_sut.IsRetryLimitExceeded());
    }

    private static PocConsumeResult<string, string> InitializeSut()
    {
        byte[] retryLimitExceededAsBytes = BitConverter.GetBytes(2);

        var headers = KafkaHeadersFactory.CreateSimpleHeader(ConsumerConstant.HEADER_NAME_RETRY_COUNT, retryLimitExceededAsBytes);

        var consumeResult = new ConsumeResult<string, string>
        {
            Message = KafkaMessageFactory.CreateKafkaMessage("test", "test", headers)
        };

        return PocConsumeResult<string, string>.Create(consumeResult, retryLimit: 3);
    }
}
