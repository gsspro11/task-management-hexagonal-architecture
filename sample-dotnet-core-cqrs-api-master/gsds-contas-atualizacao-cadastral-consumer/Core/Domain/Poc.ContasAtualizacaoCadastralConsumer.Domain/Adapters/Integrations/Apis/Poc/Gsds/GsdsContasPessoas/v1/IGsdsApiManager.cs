using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1
{
    public interface IGsdsApiManager
    {
        void SetCancellationToken(CancellationToken cancellationToken);
        Task<ConsultarContasPessoaResponse?> ConsultarContasPessoaAsync(string cpfCnpj);
    }
}
