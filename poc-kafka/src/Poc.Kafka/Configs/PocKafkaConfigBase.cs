namespace Poc.Kafka.Configs;

/// <summary>
/// Base configuration properties
/// </summary>
public abstract class PocKafkaConfigBase : PocKafkaCredentialsConfig
{
    /// <summary>
    /// Initial list of brokers as a CSV list of broker host or host:port. The application
    /// may also use `rd_kafka_brokers_add()` to add brokers during runtime. default:
    /// '' importance: high
    /// </summary>
    /// 
    public string? BootstrapServers { get; private set; }


    internal void SetBootstrapServers(string bootstrapServers) =>
        BootstrapServers = bootstrapServers;
}
