using Poc.RabbitMQ.PubSub;
using Shared;
using System.Text.Json;

namespace ProducerWorker;

public class Worker : BackgroundService
{
    private readonly IPocRabbitMQPubSub<CartaoMessage> _pubSubCartao;

    public Worker(IPocRabbitMQPubSub<CartaoMessage> pubSubCartao)
    {
        _pubSubCartao = pubSubCartao;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var serializeOpt = new JsonSerializerOptions();
        serializeOpt.Converters.Add(new CartaoConverterWithTypeDiscriminator());


        int messagesToSend = 1000;
        for (int i = 1; i <= messagesToSend; i++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            string key = i.ToString();
            await _pubSubCartao.PublishAsync(message: new CartaoMessage() { Cvv = "958", Number = "5984 4568 6485 1556" + i.ToString() },
                                  serializeOptions: serializeOpt);
        }

        _pubSubCartao.Dispose();
    }
}

