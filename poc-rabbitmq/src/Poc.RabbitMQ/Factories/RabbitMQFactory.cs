using Poc.RabbitMQ.Configs;
using Poc.RabbitMQ.PubSub;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Poc.RabbitMQ.Factories;

[ExcludeFromCodeCoverage]
internal class RabbitMQFactory<TEnumQueueName> : IDisposable, IRabbitMQFactory<TEnumQueueName>
    where TEnumQueueName : struct, Enum
{
    private readonly PocRabbitConnection _pocConnection;
    private readonly string _brokerName;
    private readonly TEnumQueueName _queue;
    private readonly ILogger<IPocRabbitMQPubSub> _logger;
    private readonly PocRabbitMQConfig? _config;
    private readonly PocRabbitMQQueueSettings? _settings;
    private IModel _channel;


    public RabbitMQFactory(
        PocRabbitConnection pocConnection,
        string brokerName,
        TEnumQueueName queue,
        ILogger<IPocRabbitMQPubSub> logger,
        PocRabbitMQConfig? config = default!,
        PocRabbitMQQueueSettings? settings = default!
    )
    {
        _pocConnection = pocConnection;
        _brokerName = brokerName;
        _queue = queue;
        _config = config;
        _settings = settings;
        _logger = logger;

        _logger.LogInformation($"RabbitMQ Channel created - Broker {_config.ClientProvidedName} - Queue {_settings.Queue}.");
    }

    public PocRabbitMQConfig Config => _config!;

    public PocRabbitMQQueueSettings Settings => _settings!;


    public RabbitMQMessage CreateRabbitMQMessage(string body, IBasicProperties properties = null)
    {
        RabbitMQMessage rabbitMQMessage = new RabbitMQMessage();

        rabbitMQMessage.Body = System.Text.Encoding.UTF8.GetBytes(body);
        rabbitMQMessage.Properties = properties;

        return rabbitMQMessage;
    }

    public void Dispose()
    {
        if (_channel is not null)
        {
            if (_channel.IsOpen)
                _channel.Close();

            _channel.Dispose();
            _channel = null;
        }

        _logger.LogInformation($"RabbitMQ Channel closed - Broker {_config.ClientProvidedName} - Queue {_settings.Queue}.");
    }

    public IModel GetChannel()
    {
        if (_channel is null || _channel.IsClosed)
            _channel = _pocConnection.Connection.CreateModel();

        return _channel;
    }

    public TEnumQueueName GetQueueEnum() => _queue;

    public string GetQueueName() => _settings.Queue;

    public ushort GetMaxConcurrentMessages() => _settings.MaxConcurrentMessages;

    public string GetBrokerName() => _brokerName;
}
