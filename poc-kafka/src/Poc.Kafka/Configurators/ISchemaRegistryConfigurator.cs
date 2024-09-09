using Confluent.SchemaRegistry;

namespace Poc.Kafka.Configurators;

internal interface ISchemaRegistryConfigurator
{
    void RegisterSchemaRegistry(string clusterName, ISchemaRegistryClient schemaRegistryClient);
}
