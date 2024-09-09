using Poc.Kafka.Factories;
using Poc.Kafka.PubSub;
using Confluent.Kafka;
using Shared;
namespace ProducerWorker;

public class Worker(
    ILogger<Worker> logger,
    [FromKeyedServices("ExampleProducer")] IPocKafkaPub<string, MessageValue> producer) : BackgroundService
{
    const string TOPIC = "TOPIC-EXAMPLE-CONSUMER";
    const string KEY = "key";

    private readonly Guid _id = Guid.NewGuid();

    private readonly MessageValue _value =
        new(Id: Guid.NewGuid().ToString(), Message: "Hello, Kafka!");

    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

    private readonly ILogger<Worker> _logger = logger;
    private readonly IPocKafkaPub<string, MessageValue> _producer = producer;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Escolha o método a ser executado:");
            Console.WriteLine("1 - SendIndividualMessageSamplesAsync");
            Console.WriteLine("2 - SendIndividualMessageWithPersonalizedDeliverySamples");
            Console.WriteLine("3 - SendBatchMessagesSamplesAsync");
            Console.WriteLine("4 - SendBatchMessagesSamples");
            Console.WriteLine("5 - SendBatchMessagesAtomicSamples");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Entrada inválida. Saindo do programa.");
                return;
            }

            switch (choice)
            {
                case 1:
                    await SendIndividualMessageSamplesAsync(stoppingToken);
                    break;
                case 2:
                    SendIndividualMessageWithPersonalizedDeliverySamples();
                    break;
                case 3:
                    await SendBatchMessagesSamplesAsync(stoppingToken);
                    break;
                case 4:
                    SendBatchMessagesSamples();
                    break;
                case 5:
                    SendBatchMessagesAtomicSamples();
                    break;
                default:
                    Console.WriteLine("Opção inválida. Saindo do programa.");
                    break;
            }
        }

        //_producer.Dispose();
    }
    private void SendBatchMessagesAtomicSamples()
    {
        var messages = CreateBatchMessages(1000);

        if (_producer.SendBatchAtomic(messages, topic: TOPIC, timeout: _timeout, batchId: _id))
        {
            _logger.LogError("Mensagens entregues com sucesso.");
        }
    }
    private async Task SendBatchMessagesSamplesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Enviando lote de mensagens...");

        var messages = CreateBatchMessages(5);

        var batchResult = await _producer.SendBatchAsync(messages, topic: TOPIC, batchId: _id, stoppingToken);
        LogBatchResultSuccesses(batchResult.Successes);
        LogBatchResultFailures(batchResult.Failures);
    }
    private void SendIndividualMessageWithPersonalizedDeliverySamples()
    {
        _logger.LogInformation("Enviando mensagem com entrega personalizada...");

        void deliveryHandler(DeliveryReport<string, MessageValue> report)
        {
            if (report.Error.IsError)
                _logger.LogInformation("Falha ao entregar a mensagem: {ErrorReason}", report.Error.Reason);
            else
                _logger.LogInformation("Mensagem entregue com sucesso: {MessageValue}", report.Message.Value);
        }

        var message = KafkaMessageFactory.CreateKafkaMessage(value: _value, key: KEY, headers: []);

        _producer.Send(value: _value, key: KEY, headers: [], topic: TOPIC, deliveryHandler);
        _producer.Send(message, topic: TOPIC, deliveryHandler);
    }
    private async Task SendIndividualMessageSamplesAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Enviando mensagem individual...");

        Guid id = Guid.NewGuid();
        var headers = KafkaHeadersFactory.CreateSimpleHeader("uuid", id.ToByteArray());

        var message = KafkaMessageFactory.CreateKafkaMessage(value: _value, key: KEY, headers);


        var deliveryResult = await _producer.SendAsync(value: _value, key: KEY, headers, topic: TOPIC, stoppingToken);
        LogDeliveryResult(deliveryResult);

        deliveryResult = await _producer.SendAsync(message, topic: TOPIC, stoppingToken);
        LogDeliveryResult(deliveryResult);
    }
    private void SendBatchMessagesSamples()
    {
        var messages = CreateBatchMessages(100);

        var batchResult = _producer.SendBatch(messages, topic: TOPIC, timeout: _timeout, batchId: _id);
        LogBatchResultSuccesses(batchResult.Successes);
        LogBatchResultFailures(batchResult.Failures);
    }
    private static List<Message<string, MessageValue>> CreateBatchMessages(int numberOfMessages)
    {
        var messages = new List<Message<string, MessageValue>>();

        for (int i = 0; i < numberOfMessages; i++)
        {
            var newGuid = Guid.NewGuid().ToString();
            var newValue = new MessageValue(Id: newGuid, Message: $"Hello, Kafka! - {i}");
            messages.Add(KafkaMessageFactory.CreateKafkaMessage(newValue, newGuid));
        }

        return messages;
    }
    private void LogDeliveryResult(DeliveryResult<string, MessageValue> deliveryResult)
    {
        _logger.LogInformation("Mensagem enviada com sucesso Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Timestamp: {Timestamp}",
            deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset, deliveryResult.Timestamp);
    }
    private void LogBatchResultFailures(List<(Message<string, MessageValue>, string)> failuresMessages)
    {
        foreach (var (_, error) in failuresMessages)
        {
            _logger.LogError("Falha ao entregar a mensagem: {Error}", error);
        }
    }
    private void LogBatchResultSuccesses(List<DeliveryResult<string, MessageValue>> successMessages)
    {
        foreach (var successMessage in successMessages)
        {
            _logger.LogInformation("Mensagem entregue com sucesso: {MessageValue}", successMessage.Message.Value);
        }
    }
}