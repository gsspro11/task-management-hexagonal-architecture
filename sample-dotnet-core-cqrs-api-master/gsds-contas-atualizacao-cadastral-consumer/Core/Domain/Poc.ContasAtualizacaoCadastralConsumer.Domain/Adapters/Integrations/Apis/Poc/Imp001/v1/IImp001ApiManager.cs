using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1
{
    public interface IImp001ApiManager
    {
        void SetCancellationToken(CancellationToken cancellationToken);
        Task<BloquearContaCorrenteResponse> BloquearContaCorrenteAsync(BloquearContaCorrente request);
    }
}
