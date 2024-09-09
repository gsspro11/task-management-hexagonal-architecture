using Poc.Kafka.Common;
using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Extensions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.Providers;

internal sealed class RetryProvider(
    ILogger<IPocKafkaPubSub> logger,
    ITimeProvider timeProvider,
    IDelayService delayService) : IRetryProvider
{
    private readonly ILogger<IPocKafkaPubSub> _logger = logger;
    private readonly ITimeProvider _timeProvider = timeProvider;
    private readonly IDelayService _delayService = delayService;

    public async Task WaitBeforeNextRetryAsync(CancellationToken cancellationToken)
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(ConsumerConstant.DELAY_BETEWEEN_RETRY_ATTEMPTS_MS);
        _logger.LogInformation("Waiting for {RetryDelayInMilliseconds} milliseconds before the next retry attempt.", delay.TotalMilliseconds);

        await _delayService.Delay(delay.Milliseconds, cancellationToken);
    }

    public bool IsRetryDelayExpired(Headers? headers = null)
    {
        try
        {
            if (headers is null)
            {
                _logger.LogInformation("Headers are null. Defaulting to immediate processing.");
                return true;
            }

            var retryAfter = GetRetryAttemptTimestamp(headers);
            TimeSpan delay = retryAfter - _timeProvider.UtcNow;
            return delay <= TimeSpan.Zero;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating retry delay. Defaulting to immediate processing.");
            return true;
        }
    }

    private static DateTimeOffset GetRetryAttemptTimestamp(Headers headers)
    {
        long milliseconds = headers.GetHeaderAs<long>(ConsumerConstant.HEADER_NAME_RETRY_AFTER);
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }
}
