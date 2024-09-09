using Poc.Kafka.Common;
using Poc.Kafka.Configs;
using Poc.Kafka.Configs.Validators;
using Poc.Kafka.Configurations;
using Poc.Kafka.Factories;
using Poc.Kafka.HealthCheck;
using Poc.Kafka.Managers;
using Poc.Kafka.Providers;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configurators;

[ExcludeFromCodeCoverage]
internal sealed class PocKafkaConfigurator : IPocKafkaConfigurator
{
    private readonly IServiceCollection _services;


    internal PocKafkaConfigurator(IServiceCollection services) =>
        _services = services;

    public IPocKafkaConfigurator AddCluster(string clusterName, Action<IClusterConfigurator> configureAction)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clusterName);

        ConfigureCommonServices();

        var clusterConfigurator = new ClusterConfigurator(clusterName, _services);
        configureAction(clusterConfigurator);

        clusterConfigurator.FinalizeConfigurations();

        return this;
    }

    private void ConfigureCommonServices()
    {
        _services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        _services.AddSingleton<IDelayService, DelayService>();
        _services.AddSingleton<IRetryProvider, RetryProvider>();
        _services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
        _services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
    }

    private sealed class ClusterConfigurator : IClusterConfigurator
    {
        private string? _bootstrapServers;
        private string? _username;
        private string? _password;

        private PocKafkaAdminClientConfig? _adminClientConfig;
        private ISchemaRegistryClient? _schemaRegistryClient;

        private readonly string _clusterName;
        private readonly IServiceCollection _services;

        private readonly HashSet<TopicConfiguration> _topicsToCreate = [];
        private readonly List<string> _topicsToCheckInMetadata = [];

        internal ClusterConfigurator(string clusterName, IServiceCollection services)
        {
            _clusterName = clusterName;
            _services = services;
        }

        public ISchemaRegistryClient SchemaRegistryClient => _schemaRegistryClient!;
        public IClusterConfigurator UseBootstrapServers(string bootstrapServers)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(bootstrapServers);

            _bootstrapServers = bootstrapServers;
            return this;
        }
        public IClusterConfigurator CreateTopicIfNotExists(string topicName, int numberOfPartitions, short replicationFactor, RetentionPeriodMs timeToRetainDataMs = RetentionPeriodMs.NoRetention)
        {
            _topicsToCreate.Add(new TopicConfiguration(topicName, numberOfPartitions, replicationFactor, timeToRetainDataMs));
            return this;
        }
        public IClusterConfigurator WithCredentials(string username, string password)
        {
            _username = username;
            _password = password;

            return this;
        }
        public IClusterConfigurator WithSchemaRegistry(Action<IPocKafkaSchemaRegistryConfig> configureAction)
        {
            var schemaRegisterConfig = ConfigureSchemaRegistry(configureAction);

            ApplyBrokerCredentialsToSchemaRegistry(schemaRegisterConfig);

            PocKafkaSchemaRegisterConfigValidator.Validate(schemaRegisterConfig);

            _schemaRegistryClient = KafkaSchemaRegistryFactory.Create(schemaRegisterConfig);

            RegisterSchemaRegistry();

            return this;
        }
        private static PocKafkaSchemaRegistryConfig ConfigureSchemaRegistry(Action<IPocKafkaSchemaRegistryConfig> configureAction)
        {
            var schemaRegisterConfig = new PocKafkaSchemaRegistryConfig();
            configureAction(schemaRegisterConfig);

            return schemaRegisterConfig;
        }
        private void ApplyBrokerCredentialsToSchemaRegistry(PocKafkaSchemaRegistryConfig schemaRegisterConfig)
        {
            if (schemaRegisterConfig.EnableUseBrokerCredentials)
                schemaRegisterConfig.SetCredentials(username: _username!, password: _password!);
        }
        private void RegisterSchemaRegistry()
        {
            var producerConfigurator = new SchemaRegistryConfigurator(_services);
            producerConfigurator.RegisterSchemaRegistry(
                clusterName: _clusterName,
                schemaRegistryClient: _schemaRegistryClient!);
        }
        public IClusterConfigurator AddProducer<TKey, TValue>(
            Action<IProducerConfigurationBuilder<TKey, TValue>> configureAction)
        {
            var producerConfiguration = ConfigureProducer(configureAction);

            _topicsToCheckInMetadata.Add(producerConfiguration.ProducerConfig.Topic!);

            RegisterProducer(producerConfiguration);

            return this;
        }
        private ProducerConfiguration<TKey, TValue> ConfigureProducer<TKey, TValue>(
            Action<IProducerConfigurationBuilder<TKey, TValue>> configureAction)
        {
            var configurationBuilder = new ProducerConfigurationBuilder<TKey, TValue>(
                bootstrapServers: _bootstrapServers!,
                username: _username,
                password: _password);

            configureAction(configurationBuilder);

            return configurationBuilder.Build();
        }
        private void RegisterProducer<TKey, TValue>(IProducerConfiguration<TKey, TValue> producerConfiguration)
        {
            var producerConfigurator = new ProducerConfigurator<TKey, TValue>(_services);
            producerConfigurator.RegisterProducer(producerConfiguration);
        }
        public IClusterConfigurator AddConsumer<TKey, TValue>(
            Action<IConsumerConfigurationBuilder<TKey, TValue>> configureAction)
        {
            var consumerConfiguration = ConfigureConsumer(configureAction);

            _topicsToCheckInMetadata.AddRange(consumerConfiguration.ConsumerConfig.Topics.Select(t => t.Topic)!);

            RegisterConsumer(consumerConfiguration);

            return this;
        }
        private ConsumerConfiguration<TKey, TValue> ConfigureConsumer<TKey, TValue>(
            Action<IConsumerConfigurationBuilder<TKey, TValue>> configureAction)
        {
            var configurationBuilder = new ConsumerConfigurationBuilder<TKey, TValue>(
                bootstrapServers: _bootstrapServers!,
                username: _username,
                password: _password);

            configureAction(configurationBuilder);

            return configurationBuilder.Build();
        }
        private void RegisterConsumer<TKey, TValue>(ConsumerConfiguration<TKey, TValue> consumerConfiguration)
        {
            var consumerConfigurator = new ConsumerConfigurator<TKey, TValue>(_services);
            consumerConfigurator.RegisterConsumer(consumerConfiguration);
        }
        internal void FinalizeConfigurations()
        {
            EnsureTopicsCreated();
            AddHealthCheck();
        }
        private void EnsureTopicsCreated()
        {
            if (_topicsToCreate.Count == 0)
                return;

            ClusterManager.CreateTopicsIfNotExists(
               config: GetAdminClientConfig(),
               topics: [.. _topicsToCreate]);
        }
        private void AddHealthCheck()
        {
            if (!ShouldAddHealthCheck())
                return;

            _services
                .AddHealthChecks()
                .AddCheck(name: $"Kafka - {_clusterName}",
                          instance: PocKafkaHealthCheck.Create(
                              adminConfig: GetAdminClientConfig(),
                              topicsToCheck: _topicsToCheckInMetadata));
        }
        private static bool ShouldAddHealthCheck()
        {
            var environment = EnvironmentHelper.GetEnvironmentName();
            ArgumentNullException.ThrowIfNull(environment);
            return !string.Equals(environment, Environments.Development, StringComparison.OrdinalIgnoreCase);
        }
        private PocKafkaAdminClientConfig GetAdminClientConfig()
        {
            if (_adminClientConfig is not null)
                return _adminClientConfig;

            _adminClientConfig = new PocKafkaAdminClientConfig();
            _adminClientConfig.SetBootstrapServers(_bootstrapServers!);

            if (_username is not null && _password is not null)
                _adminClientConfig.SetCredentials(_username, _password);

            return _adminClientConfig;
        }
    }
}