using Poc.Logging.Internal.Extensions;
using Poc.RabbitMQ;
using Poc.RabbitMQ.Extensions;
using ProducerWorker;
using Shared;
using EnumsNET;
using Poc.Project.Utils.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigurePocLoggingInternal()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddPocServiceProjectDependencies(5000);

        IConfiguration configuration = hostContext.Configuration;

        var rabbitMQBrokerSettings = configuration.GetSection(nameof(PocRabbitMQBrokerSettings)).Get<PocRabbitMQBrokerSettings>();
        foreach (var brokerSettings in rabbitMQBrokerSettings.Brokers)
        {
            var pocRabbitMQ = services.AddPocRabbitMQ<RabbitQueue>(brokerSettings.Name, brokerSettings);

            foreach (var queueSettings in brokerSettings.Queues)
                _ = pocRabbitMQ.AddPocRabbitPubSub<string>(
                    RabbitQueue.DadosClienteConta,
                    configAction: config =>
                    {
                        config.SetQueue(RabbitQueue.DadosClienteConta.AsString(EnumFormat.Description)!);
                        config.SetMaxConcurrentMessages(1000);
                    });
        }
        // services.AddHostedService<Worker>();
        services.AddHostedService<WorkerSemTipo>();
    })
    .ConfigurePocServiceProjectDependencies()
    .Build();

await host.RunAsync();