using Poc.RabbitMQ.Common;
using Poc.RabbitMQ.Configs;
using Poc.RabbitMQ.Factories;
using Poc.RabbitMQ.HealthCheck;
using Poc.RabbitMQ.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.Xml.Linq;

namespace Poc.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public class PocRabbitMQ<TEnumQueueName>
        where TEnumQueueName : struct, Enum
    {
        private readonly string _brokerName;
        private readonly IServiceCollection _services;
        private readonly PocRabbitMQConfig _config;

        public PocRabbitMQ(
            IServiceCollection services,
            string brokerName,
            Action<PocRabbitMQConfig> configAction = default!)
        {
            _brokerName = brokerName;
            _services = services;

            if (configAction is null)
            {
                throw new ArgumentException($"{nameof(configAction)} is required.");
            }

            if (configAction is not null)
            {
                _config = new PocRabbitMQConfig();
                _config.SetClientProvidedName(brokerName);
                configAction.Invoke(_config);
                PocRabbitMQValidatorConfig.ValidateConfig(_config);
            }

            services.AddSingleton<PocRabbitConnection>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<IPocRabbitMQPubSub>>();

                var connectionFactory = new ConnectionFactory();

                if (_config.IsCredentialsProvided)
                {
                    connectionFactory.UserName = _config.UserName;
                    connectionFactory.Password = _config.Password;
                }

                connectionFactory.HostName = _config.HostName;
                connectionFactory.VirtualHost = _config.VirtualHost;
                connectionFactory.Port = _config.Port;
                connectionFactory.ClientProvidedName = _config.ClientProvidedName;
                connectionFactory.AutomaticRecoveryEnabled = true;
                connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(1);


                if (_config.RequestedChannelMax > 0)
                    connectionFactory.RequestedChannelMax = _config.RequestedChannelMax;
                if (_config.RequestedFrameMax > 0)
                    connectionFactory.RequestedFrameMax = _config.RequestedFrameMax;
                if (_config.RequestedHeartbeat != TimeSpan.Zero)
                    connectionFactory.RequestedHeartbeat = _config.RequestedHeartbeat;

                if (_config.Uri != null)
                    connectionFactory.Uri = _config.Uri;

                if (_config.IsSsl)
                {
                    connectionFactory.Ssl = new SslOption
                    {
                        ServerName = _config.HostName,
                        Enabled = true
                    };
                }

                var result = new PocRabbitConnection
                {
                    Config = _config,
                    Connection = connectionFactory.CreateConnection()
                };

                logger.LogInformation($"RabbitMQ Connection created - Broker {_config.ClientProvidedName}.");

                return result;
            });

            _services.TryAddSingleton<RabbitMQConnectionResolver>(s => (brokerName) =>
            {
                var connections = s.GetRequiredService<IEnumerable<PocRabbitConnection>>();

                return connections.First(p => p.Connection.ClientProvidedName == brokerName);
            });

            var environment = EnvironmentHelpers.GetEnvironmentName();

            ArgumentNullException.ThrowIfNull(environment);

            if (!string.Equals(environment, Environments.Development, StringComparison.OrdinalIgnoreCase))
            {
                services.AddHealthChecks()
                    .AddCheck<PocRabbitMQHealthCheck>($"RabbitMQ - {_config.ClientProvidedName}");
            }
        }

        public PocRabbitMQ<TEnumQueueName> AddPocRabbitPubSub<TMessage>(TEnumQueueName queue,
                                                                        Action<PocRabbitMQQueueSettings> configAction = default!)
        {
            return AddPocRabbitMQ<IPocRabbitMQPubSub<TMessage>, PocRabbitMQPubSub<TEnumQueueName, TMessage>>(queue, configAction);
        }

        private PocRabbitMQ<TEnumQueueName> AddPocRabbitMQ<TService, TImplementation>(TEnumQueueName queue,
                                                            Action<PocRabbitMQQueueSettings> configAction = default!)
            where TService : class, IPocRabbitMQPubSub
            where TImplementation : class, TService
        {
            if (configAction is null)
                throw new ArgumentException($"{nameof(configAction)} is required.");

            var settings = new PocRabbitMQQueueSettings();

            configAction.Invoke(settings);

            _services.AddScoped<PocRabbitMQPubSubResolver<TEnumQueueName>>(s => (queue) =>
            {
                var pubSubs = s.GetRequiredService<IEnumerable<IPocRabbitMQPubSub>>();

                return pubSubs.First(p => p.GetQueueEnum() == queue.ToString());
            });

            Func<IServiceProvider, TImplementation> action = s =>
            {
                var logger = s.GetRequiredService<ILogger<IPocRabbitMQPubSub>>();

                var connectionResolver = s.GetRequiredService<RabbitMQConnectionResolver>();

                var connection = connectionResolver(_brokerName);

                return (TImplementation)Activator.CreateInstance(
                    typeof(TImplementation),
                    new RabbitMQFactory<TEnumQueueName>(Policy
                      .Handle<Exception>()
                      .WaitAndRetry(_config.RetryCount,
                        retryAttempt => TimeSpan.FromSeconds(_config.RetryIntervalSeconds), (exception, interval,
                        retryCount, context) =>
                        {
                            logger.LogError(exception, $"Error when creating RabbitMQ channel - Broker {_config.ClientProvidedName} - Queue {settings.Queue}. Attempt {retryCount}");
                        })
                      .Execute(() => connection), _brokerName, queue, logger, _config, settings),
                    logger)!;
            };

            _services.AddScoped<IPocRabbitMQPubSub, TImplementation>(action);
            _services.AddScoped<TService, TImplementation>(action);

            return this;
        }
    }
}
