using Poc.RabbitMQ.Configs;
using Poc.RabbitMQ.PubSub;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.RabbitMQ.Extensions;

[ExcludeFromCodeCoverage]
public static class ServicesCollectionExtension
{
    public static PocRabbitMQ<TEnumQueueName> AddPocRabbitMQ<TEnumQueueName>(this IServiceCollection services, string brokerName, PocRabbitMQBrokerConfig configSettings = default!)
        where TEnumQueueName : struct, Enum
    {
        return new PocRabbitMQ<TEnumQueueName>(services, brokerName, configAction: config =>
        {
            config.SetClientProvidedName(brokerName);
            config.SetCredentials(configSettings.UserName, configSettings.Password);
            config.SetServer(configSettings.HostName, configSettings.Port, configSettings.IsSsl, configSettings.RetryCount,
                configSettings.RetryIntervalSeconds, configSettings.VirtualHost);
            config.SetServerParameter(config.RequestedChannelMax, config.RequestedFrameMax,
                config.RequestedHeartbeat, config.UseBackgroundThreadsForIO);
            config.SetUri(configSettings.Uri);
            config.SetQos(config.PrefetchSizeQos, config.PrefetchCountQos, config.GlobalQos);


            if (configSettings.Queues != null && configSettings.Queues.Any())
            {
                Dictionary<string, PocRabbitMQQueueConfig> queueConfig = new Dictionary<string, PocRabbitMQQueueConfig>();

                foreach (var queueSetting in configSettings.Queues)
                {
                    var queueConfigItem = new PocRabbitMQQueueConfig
                    {
                        Queue = queueSetting.Key,
                        QueueFailed = queueSetting.Value.QueueFailed,
                    };

                    queueConfig.Add(queueSetting.Key, queueConfigItem);
                }
                config.SetQueueConfig(queueConfig);
            }
        });

    }
}
