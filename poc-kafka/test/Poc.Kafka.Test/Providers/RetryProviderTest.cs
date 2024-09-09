using Poc.Kafka.Common;
using Poc.Kafka.Common.Constants;
using Poc.Kafka.Providers;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Moq;

namespace Poc.Kafka.Test.Providers;

public class RetryProviderTest
{
    private readonly Mock<ILogger<IPocKafkaPubSub>> _mockLogger;
    private readonly Mock<ITimeProvider> _mockTimeProvider;
    private readonly Mock<IDelayService> _mockDelayService;
    private readonly RetryProvider _sut;
    public RetryProviderTest()
    {
        _mockLogger = new();
        _mockTimeProvider = new();
        _mockDelayService = new();

        _sut = InitializeSut();
    }


    [Fact]
    public void IsRetryDelayExpired_WhenHeadersIsNull_ReturnsTrue()
    {
        //Act
        bool isRetryDelayExpired = _sut.IsRetryDelayExpired(null!);

        //Assert
        Assert.True(isRetryDelayExpired);
    }

    [Theory]
    [InlineData(1000, false)]
    [InlineData(-1000, true)]
    public void IsRetryDelayExpired_GivenMillisecondsDelay_ReturnsExpectedResult(long millisecondsDelay, bool expectedRetryDelayExpired)
    {
        // Arrange
        var fixedDateTime = new DateTimeOffset(2024, 4, 1, 0, 0, 0, TimeSpan.Zero);

        var headers = new Headers();
        var retryNextAttempt = fixedDateTime.AddMilliseconds(millisecondsDelay).ToUnixTimeMilliseconds();
        headers.Add(ConsumerConstant.HEADER_NAME_RETRY_AFTER, BitConverter.GetBytes(retryNextAttempt));


        _mockTimeProvider
            .Setup(x => x.UtcNow)
            .Returns(fixedDateTime);

        // Act
        bool retryDelayExpired = _sut.IsRetryDelayExpired(headers);

        // Assert
        Assert.Equal(expectedRetryDelayExpired, retryDelayExpired);
    }

    [Fact]
    public void IsRetryDelayExpired_WhenOnException_ReturnsTrue()
    {
        // Arrange
        var headers = new Headers
        {
            { ConsumerConstant.HEADER_NAME_RETRY_AFTER, [ 0, 1, 2, 3, 4 ] }
        };

        // Act
        bool retryDelayExpired = _sut.IsRetryDelayExpired(headers);

        // Assert
        Assert.True(retryDelayExpired);
    }

    [Fact]
    public async Task WaitBeforeNextRetryAsync_WhenOnException_ReturnsTrue()
    {
        //Act
        await _sut.WaitBeforeNextRetryAsync(CancellationToken.None);

        //Assert
        _mockDelayService
             .Verify(x => x.Delay(
                 It.IsAny<int>(),
                 It.IsAny<CancellationToken>()), times: Times.Once);
    }

    private RetryProvider InitializeSut() =>
        new(logger: _mockLogger.Object,
            timeProvider: _mockTimeProvider.Object,
            delayService: _mockDelayService.Object);
}
