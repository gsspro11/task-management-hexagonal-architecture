using Poc.Kafka.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka;

/// <summary>
/// Contains extension methods for IServiceCollection to add and configure Poc.Kafka integration.
/// </summary>
[ExcludeFromCodeCoverage]
public static class PocKafka
{
    /// <summary>
    /// Registers and configures Poc.Kafka services in the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to. This collection represents the container for registering application services.</param>
    /// <param name="configureAction">An Action delegate to configure Poc.Kafka settings using a IPocKafkaConfigurationBuilder.</param>
    /// <returns>The original IServiceCollection instance, allowing for further chaining of method calls.</returns>
    public static IServiceCollection AddPocKafka(
        this IServiceCollection services,
        Action<IPocKafkaConfigurator> configureAction)
    {
        var kafkaConfigurator = new PocKafkaConfigurator(services);
        configureAction(kafkaConfigurator);
        return services;
    }
}

