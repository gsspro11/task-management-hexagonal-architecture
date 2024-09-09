using Poc.Kafka.Configs;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurations;

[ExcludeFromCodeCoverage]
internal sealed record ConsumerConfiguration<TKey, TValue> : IConsumerConfiguration<TKey, TValue>
{
    public required PocKafkaConsumerConfig ConsumerConfig { get; init; }
    public PocKafkaSerializersConfig<TKey, TValue>? SerializersConfig { get; init; }
}