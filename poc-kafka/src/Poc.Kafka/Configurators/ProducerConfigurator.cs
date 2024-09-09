using Poc.Kafka.Common;
using Poc.Kafka.Common.Extensions;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.Managers;
using Poc.Kafka.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurators;

[ExcludeFromCodeCoverage]
internal sealed class ProducerConfigurator<TKey, TValue> : IProducerConfigurator<TKey, TValue>
{
    private readonly IServiceCollection _services;
    internal ProducerConfigurator(IServiceCollection services) =>
        _services = services;
    public void RegisterProducer(IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        string serviceKey = producerConfiguration.ProducerConfig.Name!;

        if (_services.IsServiceRegistered<IPocKafkaPub<TKey, TValue>>(serviceKey))
        {
            throw new InvalidOperationException(
               $"A producer of type IPocKafkaPub<{typeof(TKey).Name}, {typeof(TValue).Name}> with key '{serviceKey}' is already registered.");
        }

        AddProducerManager(serviceKey, producerConfiguration);
        AddPocKafkaPub(serviceKey, producerConfiguration);
    }

    private void AddProducerManager(string serviceKey, IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        _services.AddKeyedSingleton<IProducerManager<TKey, TValue>>(serviceKey, (serviceProvider, key) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var producerProvider = serviceProvider.GetRequiredService<IKafkaProducerFactory>();
            return new ProducerManager<TKey, TValue>(logger, producerProvider, producerConfiguration);
        });
    }
    private void AddPocKafkaPub(string serviceKey, IProducerConfiguration<TKey, TValue> producerConfiguration)
    {
        _services.AddKeyedSingleton<IPocKafkaPub<TKey, TValue>>(serviceKey, (serviceProvider, key) =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IPocKafkaPubSub>>();
            var producerManager = serviceProvider.GetRequiredKeyedService<IProducerManager<TKey, TValue>>(key);
            return new PocKafkaPub<TKey, TValue>(logger, producerManager, producerConfiguration);
        });
    }
}