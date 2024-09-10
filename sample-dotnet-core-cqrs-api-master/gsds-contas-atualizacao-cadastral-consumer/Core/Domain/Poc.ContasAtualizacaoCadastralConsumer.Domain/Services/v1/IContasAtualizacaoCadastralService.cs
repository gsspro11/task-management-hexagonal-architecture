using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Services.v1
{
    public interface IContasAtualizacaoCadastralService
    {
        Task ProcessarAtualizacaoCadastralAsync(ContasAtualizacaoCadastralMessage message, CancellationToken cancellationToken);
    }
}
