using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.RabbitMQ
{
    public class RabbitMqClientBase<T> : IRabbitMqClientBase<T>
    {
        public static readonly int Tentativas = 30;
        public static readonly int Intervalo = 10;
        private const string _closedChannel = "O canal com o RabbitMQ encontra-se fechado";
        private readonly ILogService _logService;
        private IConnection _connection;
        private IModel _canal;
        private bool _canalExterno = false;
        public readonly QueueConfig Config;

        public RabbitMqClientBase(
            ILogService logService,
            IOptions<Dictionary<string, Dictionary<string, QueueConfig>>> configs, string chave, string chaveQueue)
        {
            _logService = logService;
            Config = configs.Value[chave][chaveQueue];
        }

        private void OpenConnection(bool criarCanal = false)
        {

            var usuarioPadrao = Config.Usuarios.ElementAt(0).Value;

            var connectionFactory = new ConnectionFactory() { DispatchConsumersAsync = true };
            connectionFactory.UserName = usuarioPadrao.Usuario;
            connectionFactory.Password = usuarioPadrao.Senha;
            connectionFactory.HostName = Config.Host;
            connectionFactory.Port = Config.Port;

            if (!string.IsNullOrWhiteSpace(Config.VirtualHost))
                connectionFactory.VirtualHost = Config.VirtualHost;

            if (!string.IsNullOrWhiteSpace(Config.Protmocolo))
                connectionFactory.Uri = new Uri($"{Config.Protocolo}://{Config.Host}:{Config.Port}");

            connectionFactory.Ssl = new SslOption
            {
                ServerName = Config.Host,
                Enabled = Config.Ssl
            };

            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

            if (_connection == null || !_connection.IsOpen)
                _connection = Policy
                                .Handle<Exception>()
                                //máximo de 1 hora de tentativas a cada 10 segundos para garantir a conexão em caso de indisponibilidade do ambiente
                                .WaitAndRetry(360, retryAttempt => TimeSpan.FromSeconds(10), (exception, interval, retryCount, context) =>
                                {
                                    _logService.Erro(exception, $"Erro ao criar a conexão junto ao RabbitMQ - host {Config.Host}. Tentativa  {retryCount}");
                                })
                                .Execute(() => connectionFactory.CreateConnection());

            _logService.Informacao($"Sucesso Conexão RabbitMq - Host {Config.Host}");
            if (criarCanal && (_canal == null || _canal.IsClosed))
                _canal = CreateChannel();


        }

        public IRabbitMqClientBase<T> OpenConnection(IModel canal = null, bool criarCanal = true)
        {
            if (canal != null && canal.IsOpen)
            {
                if (_canal != null && _canal.IsOpen)
                    _canal.Close();

                _canal = canal;
                _canalExterno = true;
            }
            else
            {
                if (canal != null && canal.IsClosed)
                    throw new RabbitMqMessageException(_closedChannel);

                if (_connection == null || !_connection.IsOpen)
                    OpenConnection(criarCanal);
                else if (criarCanal && (_canal == null || _canal.IsClosed))
                    _canal = CreateChannel();
            }

            return this;
        }

        public bool ConnectionIsOpen()
        {
            if (_connection == null)
                return false;

            return _connection.IsOpen;
        }

        public IModel CreateChannel()
        {
            if (_connection == null || !_connection.IsOpen)
                throw new RabbitMqMessageException($"A conexão com o RabbitMQ encontra-se fechada - Host {Config.Host}");

            var model = Policy
                        .Handle<Exception>()
                        .WaitAndRetry(Tentativas, retryAttempt => TimeSpan.FromSeconds(Intervalo), (exception, interval, retryCount, context) =>
                        {
                            _logService.Erro(exception, $"Erro ao criar o canal junto ao RabbitMQ - Host {Config.Host}. Tentativa {retryCount}");
                        })
                        .Execute(() => _connection.CreateModel());

            _logService.Informacao($"Sucesso criação do Channel RabbitMq - Host {Config.Host}");
            return model;



        }

        private void ValidateChannelStatus()
        {
            if (_canal == null || _canal.IsClosed)
                throw new RabbitMqMessageException(_closedChannel);
        }

        public virtual void Publish(T message)
        {
            ValidateChannelStatus();

            Policy
                .Handle<Exception>()
                .WaitAndRetry(Tentativas, retryAttempt => TimeSpan.FromSeconds(Intervalo), (exception, interval, retryCount, context) =>
                {
                    _logService.Erro(exception, $"Erro ao publicar a mensagem junto ao RabbitMQ - Host {Config.Host}. Tentativa {retryCount}");
                })
                .Execute(() =>
                {
                    var properties = _canal.CreateBasicProperties();
                    properties.Persistent = true;

                    _canal.BasicPublish("", routingKey: Config.QueueName, true, properties, body:
                        new ReadOnlyMemory<byte>(
                            Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(message))));
                });
        }

        public virtual void BatchPublish(IEnumerable<T> messages)
        {
            Policy
                .Handle<Exception>()
                .WaitAndRetry(Tentativas, retryAttempt => TimeSpan.FromSeconds(Intervalo), (exception, interval, retryCount, context) =>
                {
                    _logService.Erro(exception, $"Erro ao publicar a mensagem junto ao RabbitMQ - Host {Config.Host}. Tentativa {retryCount}");
                })
                .Execute(() =>
                {
                    var properties = _canal.CreateBasicProperties();
                    properties.Persistent = true;

                    var publishBatch = _canal.CreateBasicPublishBatch();

                    foreach (var message in messages)
                        publishBatch.Add("", routingKey: Config.QueueName, true, properties, body:
                            new ReadOnlyMemory<byte>(
                                Encoding.UTF8.GetBytes(
                                    JsonConvert.SerializeObject(message))));

                    publishBatch.Publish();
                });
        }

        public virtual void CloseConnection()
        {
            if (_canal != null && _canal.IsOpen)
            {
                _canal.Close();
                _canal.Dispose();
            }

            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public void CancelConsumer(string consumerTag)
        {
            ValidateChannelStatus();

            _canal.BasicCancel(consumerTag);
        }

        private string CreateConsumer(Func<BasicDeliverEventArgs, Task> func, ushort consumerLimit, Func<Exception, Task> exceptionHandler = null, bool restartApplication = false)
        {
            ValidateChannelStatus();

            _canal.BasicQos(0, consumerLimit, false);

            var consumer = new AsyncEventingBasicConsumer(_canal);

            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    await func.Invoke(ea);
                }
                catch (Exception ex)
                {
                    if (exceptionHandler == null)
                        throw;
                    else
                    {

                        try
                        {
                            await exceptionHandler.Invoke(ex);
                        }
                        catch (Exception exception)
                        {
                            if (restartApplication)
                                Environment.Exit(-1);
                            else
                                _logService.Erro(exception, $"Erro no invoke do consumidor junto ao RabbitMQ - Host {Config.Host}.");
                        }
                    }
                }
            };

            var consumerTag = Policy
                                .Handle<Exception>()
                                .WaitAndRetry(Tentativas, retryAttempt => TimeSpan.FromSeconds(Intervalo), (exception, interval, retryCount, context) =>
                                {
                                    _logService.Erro(exception, $"Erro ao criar o consumidor junto ao RabbitMQ - Host {Config.Host}. Tentativa {retryCount}");
                                })
                                .Execute(() => _canal.BasicConsume(queue: Config.QueueName,
                                                             autoAck: false,
                                                             consumer: consumer));

            return consumerTag;
        }

        public string CreateConsumer(Func<T, ulong, Task> func, ushort consumerLimit, Func<Exception, Task> exceptionHandler = null, bool restartApplication = false)
        {
            return CreateConsumer(async (BasicDeliverEventArgs ea) =>
            {
                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await func.Invoke(JsonConvert.DeserializeObject<T>(message), ea.DeliveryTag);
                }
                catch (Exception ex)
                {                  
                    _logService.Erro(ex, $"Erro ao tratar uma mensagem da fila via consumidor junto ao RabbitMQ - Host {Config.Host}.");

                    if (ea != null)
                        _canal.BasicNack(ea.DeliveryTag, false, true);
                }
            }, consumerLimit, exceptionHandler, restartApplication);
        }

        public IEnumerable<RabbitBasicGetResult<T>> GetQueueBlock(int messagesLimit)
        {
            var count = 0;

            while (count < messagesLimit)
            {
                var item = QueuePop();

                if (item == null)
                    break;

                count++;

                yield return item;
            }
        }

        public RabbitBasicGetResult<T> QueuePop()
        {
            ValidateChannelStatus();

            var ea = default(BasicGetResult);

            try
            {
                ea = Policy
                     .Handle<Exception>()
                     .WaitAndRetry(Tentativas, retryAttempt => TimeSpan.FromSeconds(Intervalo), (exception, interval, retryCount, context) =>
                     {
                         _logService.Erro(exception, $"Erro ao recuperar uma mensagem da fila junto ao RabbitMQ - Host {Config.Host}. Tentativa {retryCount}");
                     })
                     .Execute(() => _canal.BasicGet(queue: Config.QueueName,
                                           autoAck: false));

                if (ea == null)
                    return null;

                var result = new RabbitBasicGetResult<T>(
                    ea.DeliveryTag,
                    ea.Redelivered,
                    ea.Exchange,
                    ea.RoutingKey,
                    ea.MessageCount,
                    ea.BasicProperties,
                    ea.Body);

                var body = ea.Body.ToArray();

                var message = Encoding.UTF8.GetString(body);

                result.Object = JsonConvert.DeserializeObject<T>(message);

                return result;
            }
            catch (Exception ex)
            {
                _logService.Erro(ex, $"Erro ao recuperar uma mensagem da fila junto ao RabbitMQ. - Host {Config.Host}");

                if (ea != null)
                    _canal.BasicNack(ea.DeliveryTag, false, true);

                return null;
            }
        }

        public void BasicAck(ulong deliveryTag, bool multiple)

        {
            ValidateChannelStatus();

            _canal.BasicAck(deliveryTag, multiple);
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            ValidateChannelStatus();

            _canal.BasicNack(deliveryTag, multiple, requeue);
        }
    }
}
