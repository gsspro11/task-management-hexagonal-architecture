using Poc.Kafka.Common.Extensions;
using Poc.Kafka.PubSub;
using Poc.Kafka.Results;
using Shared;

namespace ConsumerWorker;

public class Worker(
    ILogger<Worker> logger,
    [FromKeyedServices("ExampleConsumer")] IPocKafkaSub<string, MessageValue> consumer,
    [FromKeyedServices("ExampleProducer")] IPocKafkaPub<string, MessageValue> producer) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IPocKafkaSub<string, MessageValue> _consumer = consumer;
    private readonly IPocKafkaPub<string, MessageValue> _producer = producer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker is stopping.");
        });

        try
        {
            await _consumer.ConsumeAsync((consumeResult) =>
            {
                return ProcessMessageAsync(consumeResult, stoppingToken);
            }, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");

    }
    private async Task ProcessMessageAsync(PocConsumeResult<string, MessageValue> consumeResult, CancellationToken cancellationToken)
    {
        if (consumeResult.IsRetryLimitExceeded())
        {
            // Verifica se o limite de tentativas de retry foi excedido para o fluxo de retry configurado.
            return;
        }

        //consumeResult.Topic != "TOPIC-EXAMPLE-CONSUMER-RETRY"

        if (consumeResult.Message.Value.Id == "2")
            throw new InvalidOperationException("Simulated error.");

        //if (consumeresult.message.value.code == 0)
        //{
        //    // coloca a mensagem em retry conforme necessário. invoca o .tryagain() para reprocessamento.
        //    consumeresult.tryagain();
        //    return;
        //}

        //if (consumeResult.Message.Value.Code < 0)
        //{
        //    // Envia a mensagem diretamente para a Dead Letter Queue (DLQ), invocando o método .SkipRetryAndSendToDeadLetter.
        //    consumeResult.SkipRetryAndSendToDeadLetter();
        //    return;
        //}

        // Produz uma mensagem para um tópico específico.
        //await _producer.SendAsync(consumeResult.Message, topic: "TOPIC-EXAMPLE-PRODUCER", cancellationToken);

        // Simula tempo de processamento com um delay.
        //await Task.Delay(1000, cancellationToken);

    }
}