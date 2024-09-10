using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.Autenticacao;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1
{
    public interface IGsdsAuthenticationManager
    {
        void SetCancellationToken(CancellationToken cancellationToken);
        Task<AutenticacaoResponse?> AutenticarGsdsContasPessoasAsync();
    }
}
