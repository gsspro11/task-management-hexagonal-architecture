using Confluent.Kafka;

namespace Poc.Kafka.Configs;


internal sealed class PocKafkaProducerConfig : PocKafkaConfigBase, IPocKafkaProducerConfig
{
    public string? Name { get; private set; }
    public string? Topic { get; private set; }
    public bool EnableIdempotence { get; private set; } = false;
    public bool ApiVersionRequest { get; private set; } = true;
    public int? MessageSendMaxRetries { get; private set; }
    public Acks? Acks { get; private set; }
    public int? MaxInFlight { get; private set; }
    public string? TransactionalId { get; private set; }
    public int? TransactionTimeoutMs { get; private set; }
    public int? BatchSize { get; private set; }
    public int? BatchNumMessages { get; private set; }
    public double? LingerMs { get; private set; }
    public void SetName(string name) =>
        Name = name;

    public void SetTopic(string topic) =>
        Topic = topic;

    public void SetIdempotenceEnabled()
    {
        EnableIdempotence = true;
        MaxInFlight = 5;
        MessageSendMaxRetries = 3;
        Acks = Confluent.Kafka.Acks.All;
    }

    public void SetApiVersionRequest(bool apiVersionRequest) =>
        ApiVersionRequest = apiVersionRequest;

    public void SetMessageSendMaxRetries(int messageSendMaxRetries) =>
        MessageSendMaxRetries = messageSendMaxRetries;

    public void SetAcks(Acks acks) =>
        Acks = acks;

    public void SetMaxInFlight(int maxInFlight) =>
        MaxInFlight = maxInFlight;

    public void SetTransactionalId(string transactionalId) =>
        TransactionalId = $"{transactionalId}-{Guid.NewGuid()}";

    public void SetBatchSize(int batchSize) =>
        BatchSize = batchSize;

    public void SetBatchNumMessages(int batchNumMessages) =>
        BatchNumMessages = batchNumMessages;

    public void SetLingerMs(double lingerMs) =>
       LingerMs = lingerMs;

    public void SetTransactionTimeoutMs(int transactionTimeoutMs) =>
       TransactionTimeoutMs = transactionTimeoutMs;
}
