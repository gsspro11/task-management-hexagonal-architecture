using Poc.Project.Utils.Base;
using Poc.Project.Utils.HealthCheck;
using Poc.RabbitMQ.PubSub;
using Shared;
using System.Text.Json;

namespace ProducerWorker;

public class WorkerSemTipo : PocBackgroundService
{
    private readonly ILogger<WorkerSemTipo> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WorkerSemTipo(
        ILogger<WorkerSemTipo> logger,
        PocWorkerStateService workerStateService,
        IServiceProvider serviceProvider
        ) : base(logger, workerStateService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

    }


    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker is stopping.");
        });

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var resolver = scope.ServiceProvider.GetRequiredService<PocRabbitMQPubSubResolver<RabbitQueue>>();
            var pubSubAccount = (IPocRabbitMQPubSub<string>)resolver(RabbitQueue.DadosClienteConta);

            await ExecuteAsync(pubSubAccount, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");

    }




    protected async Task ExecuteAsync(IPocRabbitMQPubSub<string> pubSubAccount, CancellationToken stoppingToken)
    {

        var serializeOpt = new JsonSerializerOptions();
        serializeOpt.Converters.Add(new CartaoConverterWithTypeDiscriminator());

        for (int j = 1; j <= 1000; j++)
        {

            for (int i = 1; i <= 1000; i++)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                await pubSubAccount.PublishAsync(message: "${ Cvv = 958, Number = '5984 4568 6485 1556' {i.ToString()}}",
                                      serializeOptions: serializeOpt);
            }

            await Task.Delay(4000, stoppingToken);

        }

        pubSubAccount.Dispose();
    }
}

