using Poc.Kafka.Configs;
using Confluent.Kafka;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurations;

[ExcludeFromCodeCoverage]
internal sealed class SerializationConfigurationBuilder<TKey, TValue> : IDeserializationConfigurationBuilder<TKey, TValue>
{
    private ISerializer<TKey>? _keySerializer;
    private ISerializer<TValue>? _valueSerializer;
    private IDeserializer<TKey>? _keyDeserializer;
    private IDeserializer<TValue>? _valueDeserializer;

    private IAsyncSerializer<TKey>? _keyAsyncSerializer;
    private IAsyncSerializer<TValue>? _valueAsyncSerializer;

    public ISerializationConfigurationBuilder<TKey, TValue> SetKeySerializer(ISerializer<TKey> serializer)
    {
        _keySerializer = serializer;
        return this;
    }

    public ISerializationConfigurationBuilder<TKey, TValue> SetValueSerializer(ISerializer<TValue> serializer)
    {
        _valueSerializer = serializer;
        return this;
    }

    public IDeserializationConfigurationBuilder<TKey, TValue> SetKeyDeserializer(IDeserializer<TKey> deserializer)
    {
        _keyDeserializer = deserializer;
        return this;
    }

    public IDeserializationConfigurationBuilder<TKey, TValue> SetValueDeserializer(IDeserializer<TValue> deserializer)
    {
        _valueDeserializer = deserializer;
        return this;
    }

    public ISerializationConfigurationBuilder<TKey, TValue> SetKeyAsyncSerializer(IAsyncSerializer<TKey> serializer)
    {
        _keyAsyncSerializer = serializer;
        return this;
    }

    public ISerializationConfigurationBuilder<TKey, TValue> SetValueAsyncSerializer(IAsyncSerializer<TValue> serializer)
    {
        _valueAsyncSerializer = serializer;
        return this;
    }

    internal PocKafkaSerializersConfig<TKey, TValue> Build() => new()
    {
        KeySerializer = _keySerializer!,
        ValueSerializer = _valueSerializer!,
        KeyDeserializer = _keyDeserializer!,
        ValueDeserializer = _valueDeserializer!,
        KeyAsyncSerializer = _keyAsyncSerializer!,
        ValueAsyncSerializer = _valueAsyncSerializer!,
    };
}
