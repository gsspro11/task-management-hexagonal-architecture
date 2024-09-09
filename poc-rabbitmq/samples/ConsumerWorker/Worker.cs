using Poc.RabbitMQ.PubSub;
using Poc.RabbitMQ.Results;
using Poc.Project.Utils.Base;
using Poc.Project.Utils.HealthCheck;
using Shared;



namespace ConsumerWorker;

public class Worker : PocBackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        ILogger<Worker> logger,
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

            await pubSubAccount.ConsumeMessageManualAckFailedRedirectAsync(onMessageReceived: ProcessMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");


    }

    private async Task<bool> ProcessMessage(PocConsumeResult<string> result)
    {
        try
        {
            _logger.LogInformation(message: "Cartao number: '{result}'", result);

            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }


        return true;
    }

}