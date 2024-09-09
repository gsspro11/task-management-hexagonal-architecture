using Poc.Kafka.Common.Constants;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.Common;

internal static class ExponentialBackoffUtility
{
    public static async Task RunWithExponentialBackoffAsync(
        Func<Task> action,
        ILogger logger,
        IDelayService delayService,
        string processName,
        CancellationToken cancellationToken)
    {
        int maxAttempts = ConsumerConstant.MAX_ATTEMPTS_ON_ERROR_CONSUMPTION;
        int attempt = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await action();
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation(ex, "{ProcessName} process was canceled.", processName);
                break;
            }
            catch (Exception ex)
            {
                attempt++;

                logger.LogError(ex, "{ProcessName} encountered an error. Attempt {Attempt} of {MaxAttempts}.",
                       processName, attempt, maxAttempts);

                if (attempt >= maxAttempts)
                {
                    logger.LogWarning("The process '{ProcessName}' reached the maximum number of consume attempts.", processName);
                    break;
                }

                int millisecondsDelay = CalculateExponentialDelayMilliseconds(attempt);

                logger.LogInformation("Waiting for {MillisecondsDelay} milliseconds before the next retry attempt.", millisecondsDelay);

                await delayService.Delay(millisecondsDelay, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private static int CalculateExponentialDelayMilliseconds(int attempt) =>
        (int)Math.Pow(2, attempt) * ConsumerConstant.DELAY_ON_ERROR_CONSUMPTION_MS;
}
