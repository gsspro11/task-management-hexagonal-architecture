using Poc.Kafka.Common;
using Poc.Kafka.Configurations;
using Poc.Kafka.Results;
using Microsoft.Extensions.Logging;

namespace Poc.Kafka.Managers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S6672:Generic logger injection should match enclosing type",
    Justification = "All logs are registered with the same type (IPocKafkaPubSub) for easy log capturing.")]
internal sealed class ConsumerManager<TKey, TValue> : IConsumerManager<TKey, TValue>
{
    private readonly ILogger<IPocKafkaPubSub> _logger;
    private readonly IDelayService _delayService;
    private readonly IConsumerManagerCore<TKey, TValue> _consumerCore;
    private readonly IConsumerConfiguration<TKey, TValue> _consumerConfiguration;

    internal ConsumerManager(
         ILogger<IPocKafkaPubSub> logger,
         IDelayService delayService,
         IConsumerManagerCore<TKey, TValue> consumerCore,
         IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _logger = logger;
        _delayService = delayService;
        _consumerCore = consumerCore;
        _consumerConfiguration = consumerConfiguration;
    }

    public async Task InitiateConsumeAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        _consumerCore.ConfigureTopicsSubscription();

        await ExecuteConsumeOperationsAsync(onMessageReceived, cancellationToken);
    }

    private async Task ExecuteConsumeOperationsAsync(
        Func<PocConsumeResult<TKey, TValue>, Task> onMessageReceived,
        CancellationToken cancellationToken)
    {
        await ExponentialBackoffUtility.RunWithExponentialBackoffAsync(async () =>
        {
            var consumeResult = _consumerCore.ConsumeMessage(cancellationToken);

            await _consumerCore.ProcessMessageAsync(consumeResult, onMessageReceived, cancellationToken);

        }, _logger, _delayService, nameof(ConsumerManager<TKey, TValue>), cancellationToken);
    }
    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Disposing ConsumerManager - {Name}.", _consumerConfiguration.ConsumerConfig.Name);
        await _consumerCore.DisposeAsync();
    }
}