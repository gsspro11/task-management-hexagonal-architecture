using Poc.Kafka.Common;
using System.Diagnostics;

namespace Poc.Kafka.Test.Common;

public class DelayServiceTest
{
    private readonly DelayService _sut;
    public DelayServiceTest() =>
        _sut = new();

    [Fact]
    public async Task Delay_WhenSpecifiedTime_WaitsAtLeastAsLongAsSpecified()
    {
        //Arrange
        var millisecondsDelay = 100;
        var stopwatch = Stopwatch.StartNew();

        // Act
        await _sut.Delay(millisecondsDelay, CancellationToken.None);

        // Assert
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds >= millisecondsDelay, "The delay was shorter than expected.");
    }

    [Fact]
    public async Task Delay_WhenCancellation_ThrowsOperationCanceledException()
    {
        var delayService = new DelayService();
        using var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel(); // Cancel immediately

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await delayService.Delay(5000, cancellationTokenSource.Token));
    }
}