using Poc.Kafka.Common.Settings;
using Poc.Logging.Internal.Extensions;
using ProducerWorker;
using Poc.Kafka;
using Shared;

IHost host = Host.CreateDefaultBuilder(args)
    .UseEnvironment("Staging")
    .ConfigureServices((hostContext, services) =>
    {
        var pocKafkaSettings = hostContext.Configuration.GetSection(nameof(PocKafkaSettings)).Get<PocKafkaSettings>();

        ArgumentNullException.ThrowIfNull(pocKafkaSettings);
        ArgumentNullException.ThrowIfNull(pocKafkaSettings.Clusters);

        var cluster = pocKafkaSettings.Clusters[0];

        services.AddPocKafka(configure =>
        {
            configure
                .AddCluster(cluster.Name, configureAction =>
                {
                    configureAction.UseBootstrapServers(cluster.BootstrapServers);
                    configureAction.CreateTopicIfNotExists("TEST-KAFKA-PRODUCER", 6, 1);
                    configureAction.AddProducer<string, MessageValue>(configureAction =>
                    {
                        configureAction
                        .Configure(configure =>
                        {
                            configure.SetName(cluster.Producers[0].Name);
                            configure.SetTopic(cluster.Producers[0].Topic);
                            //configure.SetTransactionalId(cluster.Producers[0].TransactionalId!);

                        });
                    });
                });
        });

        services.AddHostedService<Worker>();
    })
    .ConfigurePocLoggingInternal()
    .Build();

await host.RunAsync();