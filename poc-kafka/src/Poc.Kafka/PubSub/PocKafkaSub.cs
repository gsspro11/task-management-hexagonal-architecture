using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Managers;
using Poc.Kafka.Results;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.PubSub;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]

internal sealed class PocKafkaSub<TKey, TValue> : IPocKafkaSub<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly IConsumerManager<TKey, TValue> _consumerManager;
    private readonly IRetryConsumerManager<TKey, TValue> _retryManager;
    private readonly IConsumerConfiguration<TKey, TValue> _consumerConfiguration;

    internal PocKafkaSub(
        ILogger<IPocKafkaPubSub> logger,
        IRetryConsumerManager<TKey, TValue> retryManager,
        IConsumerManager<TKey, TValue> consumerManager,
        IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _logger = logger;
        _retryManager = retryManager;
        _consumerManager = consumerManager;
        _consumerConfiguration = consumerConfiguration;
    }

    public async Task ConsumeAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Kafka message consumption.");

        try
        {
            var consumerTask = _consumerManager.InitiateConsumeAsync(onMessageReceived, cancellationToken);

            if (!_consumerConfiguration.ConsumerConfig.EnableRetryTopicConsumer)
            {
                _logger.LogInformation("Retry topic subscription is disabled.");
                await consumerTask;
                return;
            }

            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var retryConsumerTask = _retryManager.InitiateConsumeAsync(onMessageReceived, linkedCancellationTokenSource.Token);

            await Task.WhenAll(consumerTask, retryConsumerTask);

            _logger.LogInformation("All consumers ran successfully.");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Consumers were canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Consumer(s) attempts failed.");
        }
        finally
        {
            await DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing PocKafkaSub - {Name}.", _consumerConfiguration.ConsumerConfig.Name);

        await _retryManager.DisposeAsync();
        await _consumerManager.DisposeAsync();
    }
}