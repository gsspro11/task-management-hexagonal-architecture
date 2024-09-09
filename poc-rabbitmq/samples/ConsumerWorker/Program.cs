using Poc.Logging.Internal.Extensions;
using Poc.Project.Utils;
using Poc.Project.Utils.Extensions;
using Poc.RabbitMQ;
using Poc.RabbitMQ.Extensions;
using ConsumerWorker;
using EnumsNET;
using Shared;


PocProjectUtils.SetProjectExecutionFolder();

IHost host = Host.CreateDefaultBuilder(args)
           .UseDefaultServiceProvider(config =>
           {
               config.ValidateScopes = true;
               config.ValidateOnBuild = true;
           })
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
                               config.SetQueueFailed(queueSettings.Value.QueueFailed);
                               config.SetMaxConcurrentMessages(1000);
                           });
               }

               services.AddHostedService<Worker>();
           })
           .ConfigurePocServiceProjectDependencies()
           .Build();

await host.RunAsync();
