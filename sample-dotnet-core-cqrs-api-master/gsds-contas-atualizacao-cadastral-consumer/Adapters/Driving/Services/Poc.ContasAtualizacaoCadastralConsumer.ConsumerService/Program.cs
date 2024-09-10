using Poc.Logging.Internal.Extensions;
using Poc.Api.Client.Extensions;
using Poc.Project.Utils.Extensions;
using Poc.Parameter.Manager.Extensions;
using Poc.ContasAtualizacaoCadastralConsumer.Application;
using Poc.Kafka.Extensions;
using Poc.Kafka;
using EnumsNET;
using Poc.Project.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums.Kafka;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService;
using Poc.Kafka.Configs;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds;

namespace Poc.ContasAtualizacaoCadastralConsumer.Service
{
    public static class Program
    {
        private const string ApplicationName = "gsds";

        public static Task Main(string[] args)
        {
            PocProjectUtils.SetProjectExecutionFolder();

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigurePocParameterManager(ApplicationName)
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    var env = context.HostingEnvironment.EnvironmentName.ToString().ToLower();
                    var nameService = context.Configuration.GetValue<string>("ContasAtualizacaoCadastralConsumer.ConsumerService:Name")?.ToString().ToLower() ?? "nao-definido";

                    var groupId = $"{nameService}-{env}";

                    Console.WriteLine($"Application: {groupId} - started");

                    services.AddPocServiceProjectDependencies(configuration.GetValue<int>("ContasAtualizacaoCadastralConsumer.ConsumerService:ProcessTimeoutMs"));
                    services.AddPocApiClientService(configuration.GetValue<int>("ContasAtualizacaoCadastralConsumer.ConsumerService:ApiConsumptionTimeoutMs"));

                    services.AddApplicationModule();
                    services.AddGsdsModule(configuration);
                    services.AddImp001Module(configuration);

                    KafkaConfiguration(context, services, groupId);

                    services.AddHostedService<Worker>();
                })
                .ConfigurePocServiceProjectDependencies()
                .ConfigurePocLoggingInternal()
            .Build();

            return host.RunAsync();
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        private static void KafkaConfiguration(HostBuilderContext context, IServiceCollection services, string groupId)
        {
            var userKafka = context.Configuration.GetValue<string>("UsernameKafkaCorp")?.ToString() ?? null;
            var passKafka = context.Configuration.GetValue<string>("PasswordKafkaCorp")?.ToString() ?? null;

            var kafkaSettings = context.Configuration.GetSection("PocKafkaBrokerSettings:BrokerCorp").Get<PocKafkaBrokerSettings>();

            var kafka = (userKafka is not null && passKafka is not null) ?
                   services.AddPocKafka<KafkaTopic>("BrokerCorp", kafkaSettings!.BootstrapServer, userKafka, passKafka) :
                   services.AddPocKafka<KafkaTopic>("BrokerCorp", kafkaSettings!.BootstrapServer);

            foreach (var topic in kafkaSettings.Topics
                                .Where(x => x.Key.Equals(KafkaTopic.CaduNotificaAlteracaoStatusCadastral.AsString(EnumFormat.Description),
                                        StringComparison.OrdinalIgnoreCase)))
            {
                kafka.AddPocKafkaPubSub<string, ContasAtualizacaoCadastralMessage>(
                                    KafkaTopic.CaduNotificaAlteracaoStatusCadastral,
                                    subConfigAction: GetPocKafkaSubConfigAction(topic, GetJsonSerializerOptions(), groupId));
            }
        }
        private static Action<PocKafkaSubConfig> GetPocKafkaSubConfigAction(KeyValuePair<string, PocKafkaTopicSettings> topic,
            JsonSerializerOptions jsonOptions, string groupId)
        {
            var sessionTimeoutMs = topic.Value.SessionTimeoutMs == 0 ? 45000 : topic.Value.SessionTimeoutMs;

            return config =>
            {
                config.SetAutoOffsetReset(Confluent.Kafka.AutoOffsetReset.Earliest);

                config.SetTopic(topic.Key);
                config.SetGroupId(groupId);
                config.SetMaxConcurrentMessages(topic.Value.MaxConcurrentMessages!);
                config.SetSessionTimeoutMs(sessionTimeoutMs);
                config.SetJsonSerializerOptionsValue(jsonOptions);
            };
        }
    }
}