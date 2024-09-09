using Poc.Kafka.Common;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.Providers;
using Poc.Kafka.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurators;

[ExcludeFromCodeCoverage]
internal sealed class ConsumerConfigurator<TKey, TValue> : IConsumerConfigurator<TKey, TValue>
{
    private readonly IServiceCollection _services;
    internal ConsumerConfigurator(IServiceCollection services) =>
        _services = services;

    public void RegisterConsumer(IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        string serviceKey = consumerConfiguration.ConsumerConfig.Name!;

        if (_services.IsServiceRegistered<IPocKafkaSub<TKey, TValue>>(serviceKey))
        {
            throw new InvalidOperationException(
               $"A consumer of type IPocKafkaSub<{typeof(TKey).Name}, {typeof(TValue).Name}> with key '{serviceKey}' is already registered.");
        }

        AddConsumerCore(serviceKey, consumerConfiguration);
        AddRetryConsumerManager(serviceKey, consumerConfiguration);
        AddConsumerManager(serviceKey, consumerConfiguration);
        AddPocKafkaSub(serviceKey, consumerConfiguration);
    }


    private void AddConsumerCore(string serviceKey, IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _services.AddKeyedTransient<IConsumerManagerCore<TKey, TValue>>(serviceKey, (serviceProvider, key) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var timeProvider = serviceProvider.GetRequiredService<ITimeProvider>();
            var delayService = serviceProvider.GetRequiredService<IDelayService>();
            var consumerFactory = serviceProvider.GetRequiredService<IKafkaConsumerFactory>();
            var producerFactory = serviceProvider.GetRequiredService<IKafkaProducerFactory>();

            return new ConsumerManagerCore<TKey, TValue>(
                logger,
                timeProvider,
                delayService,
                consumerFactory,
                producerFactory,
                consumerConfiguration);
        });
    }
    private void AddRetryConsumerManager(string serviceKey, IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _services.AddKeyedSingleton<IRetryConsumerManager<TKey, TValue>>(serviceKey, (serviceProvider, key) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var retryProvider = serviceProvider.GetRequiredService<IRetryProvider>();
            var delayService = serviceProvider.GetRequiredService<IDelayService>();
            var consumerCore = serviceProvider.GetRequiredKeyedService<IConsumerManagerCore<TKey, TValue>>(key);

            return new RetryConsumerManager<TKey, TValue>(
                logger,
                retryProvider,
                delayService,
                consumerCore,
                consumerConfiguration);
        });
    }
    private void AddConsumerManager(string serviceKey, IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _services.AddKeyedSingleton<IConsumerManager<TKey, TValue>>(serviceKey, (serviceProvider, key) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var delayService = serviceProvider.GetRequiredService<IDelayService>();
            var consumerCore = serviceProvider.GetRequiredKeyedService<IConsumerManagerCore<TKey, TValue>>(key);

            return new ConsumerManager<TKey, TValue>(
                logger,
                delayService,
                consumerCore,
                consumerConfiguration);
        });
    }
    private void AddPocKafkaSub(string serviceKey, IConsumerConfiguration<TKey, TValue> consumerConfiguration)
    {
        _services.AddKeyedSingleton<IPocKafkaSub<TKey, TValue>>(serviceKey, (serviceProvider, key) => {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var retryManager = serviceProvider.GetRequiredKeyedService<IRetryConsumerManager<TKey, TValue>>(key);
            var consumerManager = serviceProvider.GetRequiredKeyedService<IConsumerManager<TKey, TValue>>(key);

            return new PocKafkaSub<TKey, TValue>(
                logger,
                retryManager,
                consumerManager,
                consumerConfiguration);

        });
    }
}
