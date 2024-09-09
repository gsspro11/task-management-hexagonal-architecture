using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Providers;
using Poc.Kafka.Results;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.Managers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]

internal sealed class RetryConsumerManager<TKey, TValue> : IRetryConsumerManager<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly IDelayService _delayService;
    private readonly IRetryProvider _retryProvider;
    private readonly IConsumerManagerCore<TKey, TValue> _consumerCore;
    private readonly IConsumerConfiguration<TKey, TValue> _consumerConfiguration;

    internal RetryConsumerManager(
         ILogger<IPocKafkaPubSub> logger,
         IRetryProvider retryProvider,
         IDelayService delayService,
         IConsumerManagerCore<TKey, TValue> consumerCore,
         IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _logger = logger;
        _delayService = delayService;
        _retryProvider = retryProvider;
        _consumerCore = consumerCore;
        _consumerConfiguration = consumerConfiguration;
    }


    public async Task InitiateConsumeAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        _consumerCore.Subscribe([_consumerConfiguration.ConsumerConfig.TopicRetry!]);

        await ExecuteConsumeOperationsAsync(onMessageReceived, cancellationToken);
    }

    private async Task ExecuteConsumeOperationsAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {

        await ExponentialBackoffUtility.RunWithExponentialBackoffAsync(async () =>
        {
            var consumeResult = _consumerCore.ConsumeMessage(cancellationToken);

            if (_retryProvider.IsRetryDelayExpired(consumeResult.Message?.Headers))
                await _consumerCore.ProcessMessageAsync(consumeResult, onMessageReceived, cancellationToken);
            else
            {
                await _retryProvider.WaitBeforeNextRetryAsync(cancellationToken).ConfigureAwait(false);

                var topicPartitionOffset = KafkaTopicPartitionOffsetFactory.Create(consumeResult);
                _consumerCore.Seek(topicPartitionOffset);
            }

        }, _logger, _delayService, nameof(RetryConsumerManager<TKey, TValue>), cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing ConsumerManager - {Name}.", _consumerConfiguration.ConsumerConfig.Name);
        await _consumerCore.DisposeAsync();
    }
}
