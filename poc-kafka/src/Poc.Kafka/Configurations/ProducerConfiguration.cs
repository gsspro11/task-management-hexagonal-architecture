using Poc.Kafka.Configs;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurations;

[ExcludeFromCodeCoverage]
internal sealed record ProducerConfiguration<TKey, TValue> : IProducerConfiguration<TKey, TValue>
{
    public required PocKafkaProducerConfig ProducerConfig { get; init; }
    public PocKafkaSerializersConfig<TKey, TValue>? SerializersConfig { get; init; }
}