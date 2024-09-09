namespace Poc.Kafka.Common;

internal interface IDelayService
{
    Task Delay(int millisecondsDelay, CancellationToken cancellationToken);
}
