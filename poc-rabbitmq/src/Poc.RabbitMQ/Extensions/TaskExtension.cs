using Poc.RabbitMQ.PubSub;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Poc.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
public static class TaskExtension
{
    public static async Task<T> RetryAsync<T>(this Func<Task<T>> taskFunc, int maxRetries, TimeSpan delayBetweenRetries, CancellationToken cancellationToken, ILogger<IPocRabbitMQPubSub> logger = default!)
    {
        int currentRetry = 0;
        while (currentRetry <= maxRetries)
        {
            try
            {
                return await taskFunc();
            }
            catch (Exception ex)
            {
                if (currentRetry >= maxRetries) throw;

                currentRetry++;
                logger?.LogInformation("Error encountered. Retry {CurrentRetry} of {MaxRetries}. Exception: {Message}", currentRetry, maxRetries, ex.Message);

                await Task.Delay(delayBetweenRetries, cancellationToken);
            }
        }

        throw new InvalidOperationException("Failed after maximum retries.");
    }

}
