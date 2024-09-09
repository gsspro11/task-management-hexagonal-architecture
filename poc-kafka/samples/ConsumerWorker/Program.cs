using Poc.Kafka;
using Poc.Kafka.Common;
using Poc.Kafka.Common.Settings;
using Poc.Kafka.Configurators;
using Poc.Logging.Internal.Extensions;
using Confluent.Kafka;
using ConsumerWorker;
using Shared;

IHost host = Host.CreateDefaultBuilder(args)
           .UseEnvironment("Staging")
           .ConfigureServices((hostContext, services) =>
           {
               var pocKafkaSettings = hostContext.Configuration.GetSection(nameof(PocKafkaSettings)).Get<PocKafkaSettings>();

               ArgumentNullException.ThrowIfNull(pocKafkaSettings);
               ArgumentNullException.ThrowIfNull(pocKafkaSettings.Clusters);

               services.AddPocKafka(configure =>
               {
                   foreach (var cluster in pocKafkaSettings.Clusters)
                   {
                       configure
                       .AddCluster(cluster.Name!, configureCluster =>
                       {
                           configureCluster.UseBootstrapServers(cluster.BootstrapServers!);
                           configureCluster.CreateTopicIfNotExists("TEST-KAFKA-CONSUMER", 6, 1, RetentionPeriodMs.OneDay);
                           configureCluster.CreateTopicIfNotExists("TEST-KAFKA-PRODUCER", 6, 1);
                           //configureCluster.WithSchemaRegistry(configure =>
                           //{
                           //    configure.SetUrl(url: cluster.SchemaRegistrySettings!.Url);
                           //    configure.UseBrokerCredentials();
                           //});

                           foreach (var consumer in cluster.Consumers)
                               ConfigureConsumer(configureCluster, consumer);

                           foreach (var producer in cluster.Producers)
                               ConfigureProducer(configureCluster, producer);
                       });
                   }
               });

               services.AddHostedService<Worker>();
           })
           .ConfigurePocLoggingInternal()
           .Build();


static void ConfigureConsumer(IClusterConfigurator clusterConfigurator, PocConsumerSettings consumerConfig)
{
    switch (consumerConfig.Name)
    {
        case nameof(KafkaTopic.ExampleConsumer):
            clusterConfigurator.AddConsumer<string, MessageValue>(configureConsumer =>
            {
                configureConsumer
                .Configure(configure =>
                {
                    configure.SetName(consumerConfig.Name);
                    configure.SetTopics(consumerConfig.Topics);
                    configure.SetGroupId(consumerConfig.GroupId);
                    configure.SetAutoOffsetReset(AutoOffsetReset.Earliest);
                    configure.SetDelayPartitionEofMs(consumerConfig.DelayPartitionEofMs!.Value);
                    configure.SetEnableRetryTopicConsumer();
                    configure.SetRetryLimit(consumerConfig.RetryLimit!.Value);
                    configure.SetRetryDelayMs(consumerConfig.RetryDelayMs!.Value);
                    configure.SetTopicRetry(consumerConfig.TopicRetry!);
                    configure.SetTopicDeadLetter(consumerConfig.TopicDeadLetter!);
                    //configure.SetMaxConcurrentMessages(consumerConfig.MaxConcurrentMessages!.Value);
                    //configure.SetEnableAutoCommit();
                });
            });
            break;
        default:
            break;
    }
}

static void ConfigureProducer(IClusterConfigurator clusterConfigurator, PocProducerSettings producerConfig)
{
    switch (producerConfig.Name)
    {
        case nameof(KafkaTopic.ExampleProducer):
            clusterConfigurator.AddProducer<string, MessageValue>(configureProducer =>
            {
                configureProducer.Configure(configure =>
                {
                    configure.SetName(producerConfig.Name);
                    configure.SetTopic(producerConfig.Topic);
                });
            });
            break;
        default:
            break;
            // Outros cases para diferentes tópicos...
    }
}


await host.RunAsync();
