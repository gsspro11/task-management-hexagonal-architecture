using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurators;

[ExcludeFromCodeCoverage]
internal sealed class SchemaRegistryConfigurator : ISchemaRegistryConfigurator
{
    private readonly IServiceCollection _services;
    internal SchemaRegistryConfigurator(IServiceCollection services) =>
        _services = services;

    public void RegisterSchemaRegistry(string clusterName, ISchemaRegistryClient schemaRegistryClient) =>
        _services.AddKeyedSingleton(clusterName, schemaRegistryClient);
}
