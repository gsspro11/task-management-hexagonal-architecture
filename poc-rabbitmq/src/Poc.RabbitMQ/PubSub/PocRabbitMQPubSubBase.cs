using Poc.RabbitMQ.Extensions;
using Poc.RabbitMQ.Factories;
using Poc.RabbitMQ.Results;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

namespace Poc.RabbitMQ.PubSub;
internal abstract class PocRabbitMQPubSubBase<TEnumQueueName, TMessage> : IPocRabbitMQPubSub<TMessage>
    where TEnumQueueName : struct, Enum
{
    private readonly IRabbitMQFactory<TEnumQueueName> _rabbitMQFactory;
    private readonly ILogger<IPocRabbitMQPubSub> _logger;

    private IModel GetChannel() => _rabbitMQFactory.GetChannel();
    private void RejectWithoutError(BasicDeliverEventArgs ea, IModel channel)
    {
        if (ea != null && channel != null && channel.IsOpen) return;

        try
        {
            channel.BasicReject(ea.DeliveryTag, false);
        }
        catch (Exception exx)
        {
            _logger.LogError(exx,
                $"Error when consuming a message from RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");
        }
    }

    protected PocRabbitMQPubSubBase(
        IRabbitMQFactory<TEnumQueueName> rabbitMqFactory,
        ILogger<IPocRabbitMQPubSub> logger)
    {
        _rabbitMQFactory = rabbitMqFactory;
        _logger = logger;
    }

    public async Task PublishAsync(TMessage message, IBasicProperties props = null, bool mandatory = true,
        JsonSerializerOptions serializeOptions = null)
    {
        Policy
            .Handle<Exception>()
            .WaitAndRetry(_rabbitMQFactory.Config.RetryCount, retryAttempt => TimeSpan.FromSeconds(
                _rabbitMQFactory.Config.RetryIntervalSeconds), (exception, interval, retryCount, context) =>
                {
                    _logger.LogError(exception,
                        $"Error when sending a message to RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}. Attempt {retryCount}");
                })
            .Execute(() =>
            {
                string messageStr = string.Empty;
                IModel channel = GetChannel();
                props ??= channel.CreateBasicProperties();


                if (Type.GetTypeCode(typeof(TMessage)) == TypeCode.Object)
                    messageStr = message.Serialize(serializeOptions);
                else
                    messageStr = message.ToString();

                channel.BasicPublish(string.Empty, _rabbitMQFactory.GetQueueName(), mandatory, props, new ReadOnlyMemory<byte>(
                        Encoding.UTF8.GetBytes(messageStr)));
            });
    }

    public async Task BatchPublishAsync(IEnumerable<TMessage> messages, IBasicProperties props = null, bool mandatory = true,
        JsonSerializerOptions serializeOptions = null)
    {
        Policy
           .Handle<Exception>()
           .WaitAndRetry(_rabbitMQFactory.Config.RetryCount, retryAttempt => TimeSpan.FromSeconds(
                _rabbitMQFactory.Config.RetryIntervalSeconds), (exception, interval, retryCount, context) =>
                {
                    _logger.LogError(exception,
                        $"Error when sending a message to RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}. Attempt {retryCount}");
                })
            .Execute(() =>
            {
                var publishBatch = GetChannel().CreateBasicPublishBatch();

                foreach (var message in messages)
                {
                    string messageStr = string.Empty;

                    if (Type.GetTypeCode(typeof(TMessage)) == TypeCode.Object)
                        messageStr = JsonExtension.Serialize(message, serializeOptions);
                    else
                        messageStr = message.ToString();

                    publishBatch.Add(string.Empty, _rabbitMQFactory.GetQueueName(), mandatory, props, new ReadOnlyMemory<byte>(
                            Encoding.UTF8.GetBytes(messageStr)));
                }
                publishBatch.Publish();
            });
    }

    private void RegisterCancelConsumer(string consumerTag, IModel channel, CancellationToken cancellationToken)
    {
        cancellationToken.Register(() =>
        {
            channel.BasicCancel(consumerTag);
            _logger.LogInformation($"RabbitMQ Consumer {consumerTag} canceled - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");
        });
    }

    public async Task ConsumeMessageAsync(
        Func<PocConsumeResult<TMessage>, Task> onMessageReceived,
        JsonSerializerOptions serializeOptions = null,
        CancellationToken cancellationToken = default!)
    {
        IModel channel = GetChannel();
        channel.BasicQos(0, _rabbitMQFactory.GetMaxConcurrentMessages(), false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ch, ea) =>
        {
            TMessage message;

            try
            {
                byte[] body = ea.Body.ToArray();

                if (Type.GetTypeCode(typeof(TMessage)) == TypeCode.Object)
                    message = JsonExtension.Deserialize<TMessage>(body, serializeOptions);
                else
                    message = (TMessage)Convert.ChangeType(Encoding.UTF8.GetString(body), typeof(TMessage));

                await onMessageReceived.Invoke(new PocConsumeResult<TMessage>(ea.ConsumerTag, ea.DeliveryTag, ea.Redelivered,
                    ea.Exchange, ea.RoutingKey, message));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error when consuming a message from RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

                if (ea != null)
                    channel.BasicAck(ea.DeliveryTag, true);
            }
        };

        string consumerTag = channel.BasicConsume(_rabbitMQFactory.GetQueueName(), true, consumer);

        _logger.LogInformation($"RabbitMQ Consumer {consumerTag} created - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

        RegisterCancelConsumer(consumerTag, channel, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }


    public async Task ConsumeMessageManualAckAsync(
        Func<PocConsumeResult<TMessage>, Task<bool>> onMessageReceived,
        JsonSerializerOptions serializeOptions = null,
        CancellationToken cancellationToken = default!)
    {
        IModel channel = GetChannel();
        channel.BasicQos(0, _rabbitMQFactory.GetMaxConcurrentMessages(), false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ch, ea) =>
        {
            TMessage message;

            try
            {
                byte[] body = ea.Body.ToArray();

                if (Type.GetTypeCode(typeof(TMessage)) == TypeCode.Object)
                    message = JsonExtension.Deserialize<TMessage>(body, serializeOptions);
                else
                    message = (TMessage)Convert.ChangeType(Encoding.UTF8.GetString(body), typeof(TMessage));

                var result = await onMessageReceived.Invoke(new PocConsumeResult<TMessage>(ea.ConsumerTag, ea.DeliveryTag, ea.Redelivered,
                    ea.Exchange, ea.RoutingKey, message));

                if (result)
                    channel.BasicAck(ea.DeliveryTag, true);
                else
                    channel.BasicReject(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error when consuming a message from RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

                if (ea != null)
                    channel.BasicReject(ea.DeliveryTag, false);
            }
        };

        string consumerTag = channel.BasicConsume(_rabbitMQFactory.GetQueueName(), false, consumer);

        _logger.LogInformation($"RabbitMQ Consumer {consumerTag} created - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

        RegisterCancelConsumer(consumerTag, channel, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public async Task ConsumeMessageManualAckFailedRedirectAsync(
        Func<PocConsumeResult<TMessage>, Task<bool>> onMessageReceived,
        JsonSerializerOptions serializeOptions = null,
        int? retryCountConsumer = null,
        CancellationToken cancellationToken = default!)
    {
        IModel channel = GetChannel();
        channel.BasicQos(0, _rabbitMQFactory.GetMaxConcurrentMessages(), false);

        retryCountConsumer ??= _rabbitMQFactory.Config.RetryCount;

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ch, ea) =>
        {
            TMessage message;

            try
            {
                byte[] body = ea.Body.ToArray();

                if (Type.GetTypeCode(typeof(TMessage)) == TypeCode.Object)
                    message = JsonExtension.Deserialize<TMessage>(body, serializeOptions);
                else
                    message = (TMessage)Convert.ChangeType(Encoding.UTF8.GetString(body), typeof(TMessage));


                var result = await onMessageReceived.Invoke(new PocConsumeResult<TMessage>(ea.ConsumerTag, ea.DeliveryTag, ea.Redelivered,
                    ea.Exchange, ea.RoutingKey, message));

                if (result)
                    channel.BasicAck(ea.DeliveryTag, false);
                else
                {
                    int xdeathCount = 0;

                    if (ea.BasicProperties?.Headers?["x-death"] != null)
                    {
                        List<object> listXdeath = (List<object>)ea.BasicProperties.Headers["x-death"];
                        var objectXdeath = listXdeath.FirstOrDefault(item => Encoding.UTF8.GetString(
                            (byte[])(((IDictionary<string, object>)item)["queue"])).ToLower() == _rabbitMQFactory.GetQueueName().ToLower());

                        if (objectXdeath != null)
                            xdeathCount = Convert.ToInt16(((IDictionary<string, object>)objectXdeath)["count"]);
                    }


                    if (xdeathCount >= retryCountConsumer)
                    {
                        channel.BasicPublish(string.Empty, $"{_rabbitMQFactory.Settings.QueueFailed}", ea.BasicProperties, body);
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                        channel.BasicReject(ea.DeliveryTag, false);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error when consuming a message from RabbitMQ - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

                RejectWithoutError(ea, channel);
            }
        };

        string consumerTag = channel.BasicConsume(_rabbitMQFactory.GetQueueName(), false, consumer);

        _logger.LogInformation($"RabbitMQ Consumer {consumerTag} created - Broker {_rabbitMQFactory.Config.ClientProvidedName}.");

        RegisterCancelConsumer(consumerTag, channel, cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public string GetQueueEnum() =>
        _rabbitMQFactory.GetQueueEnum().ToString();

    public void Dispose()
    {
        _rabbitMQFactory.Dispose();
    }
}


