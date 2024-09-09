namespace Poc.Kafka.Common;

internal sealed class DelayService : IDelayService
{
    public async Task Delay(int millisecondsDelay, CancellationToken cancellationToken) =>
        await Task.Delay(millisecondsDelay, cancellationToken);

}