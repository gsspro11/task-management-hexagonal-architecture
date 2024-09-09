using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configs;

[ExcludeFromCodeCoverage]
internal sealed record PocKafkaSerializersConfig<TKey, TValue>
{
    public IDeserializer<TKey>? KeyDeserializer { get; init; }
    public IDeserializer<TValue>? ValueDeserializer { get; init; }
    public ISerializer<TKey>? KeySerializer { get; init; }
    public ISerializer<TValue>? ValueSerializer { get; init; }
    public IAsyncSerializer<TKey>? KeyAsyncSerializer { get; init; }
    public IAsyncSerializer<TValue>? ValueAsyncSerializer { get; init; }
}