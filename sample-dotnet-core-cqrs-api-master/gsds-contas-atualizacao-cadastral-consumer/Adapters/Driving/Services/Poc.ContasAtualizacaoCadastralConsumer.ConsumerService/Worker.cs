using Poc.Project.Utils.Base;
using Poc.Project.Utils.HealthCheck;
using Poc.Kafka.PubSub;
using Poc.Kafka.Results;
using Newtonsoft.Json;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums.Kafka;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Services.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Service
{
    public class Worker : PocBackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IPocKafkaPubSub<string, ContasAtualizacaoCadastralMessage> _pubSubContasAtualizacaoCadastral;

        public Worker(
            IServiceScopeFactory scopeFactory,
            ILogger<Worker> logger,
            PocKafkaPubSubResolver<KafkaTopic> pubSubResolver,
            PocWorkerStateService workerStateService
        ) : base(logger, workerStateService)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            _pubSubContasAtualizacaoCadastral = (IPocKafkaPubSub<string, ContasAtualizacaoCadastralMessage>)pubSubResolver(KafkaTopic.CaduNotificaAlteracaoStatusCadastral);
        }

        protected override Task ExecuteServiceAsync(CancellationToken stoppingToken)
        {
            return _pubSubContasAtualizacaoCadastral.ConsumeAsync(consumeResult =>
                ExecuteProcessAsync(consumeResult, WorkerProcess(stoppingToken)), stoppingToken);
        }

        public Func<PocConsumeResult<string, ContasAtualizacaoCadastralMessage>, Task> WorkerProcess(CancellationToken cancellationToken)
        {
            return async consumeResult =>
            {
                try
                {
                    if (consumeResult?.Message?.Value is null)
                    {
                        _logger.LogError("Erro mensagem nula");
                        return;
                    }

                    var message = consumeResult.Message.Value;

                    using var scope = _scopeFactory.CreateScope();

                    var contasAtualizacaoCadastralService = scope.ServiceProvider.GetRequiredService<IContasAtualizacaoCadastralService>();

                    LogWorkerInteractions(consumeResult, message);

                    await contasAtualizacaoCadastralService.ProcessarAtualizacaoCadastralAsync(message!, cancellationToken);

                    return;
                }
                catch (Exception ex)
                {
                    LogWorkerError(consumeResult, ex);

                    WorkerStateService.Degraded(ex.Message);

                    return;
                }
            };
        }

        private void LogWorkerError(PocConsumeResult<string, ContasAtualizacaoCadastralMessage> consumeResult, Exception ex)
        {
            _logger.LogError(ex, "Exceção {Type}: {Message}. Mensagem: {Value}.",
                                    ex.GetType().Name, ex.Message, JsonConvert.SerializeObject(consumeResult.Message.Value));
        }
        private void LogWorkerInteractions(PocConsumeResult<string, ContasAtualizacaoCadastralMessage> consumeResult, ContasAtualizacaoCadastralMessage message)
        {
            _logger.LogInformation(@"ConsumeResult: {Offset}. Message: {Message}",
                    consumeResult.TopicPartitionOffset.ToString(), JsonConvert.SerializeObject(message));
        }
    }
}