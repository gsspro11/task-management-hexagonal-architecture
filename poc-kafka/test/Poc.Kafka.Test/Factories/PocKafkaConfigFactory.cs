using Poc.Kafka.Configs;

namespace Poc.Kafka.Test.Factories;

internal static class PocKafkaConfigFactory
{
    public static PocKafkaPubConfig CreatePocKafkaPubConfig(Action<PocKafkaPubConfig> pubConfigAction)
    {
        PocKafkaPubConfig config = new();
        pubConfigAction.Invoke(config);
        return config;
    }

    public static PocKafkaSubConfig CreatePocKafkaSubConfig(Action<PocKafkaSubConfig> subConfigAction)
    {
        PocKafkaSubConfig config = new();
        subConfigAction.Invoke(config);
        return config;
    }
}
