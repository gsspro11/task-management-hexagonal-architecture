using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Extensions.v1
{
    public static class ContaExtensions
    {
        public static BloquearContaCorrente BuildBloquearContaCorrenteRequest(this Conta value, string loginWs, string senhaWs, string urlWs)
        {
            return new BloquearContaCorrente
            {
                parametros = new Parametros
                {
                    LoginWs = loginWs,
                    SenhaWs = senhaWs,
                    UrlWs = urlWs,
                    IdOrigemIntegracao = 42,

                    Agencia = (short)value.DadosRetornaContaCorrente.Agencia,
                    NumeroConta = value.DadosRetornaContaCorrente.NumeroConta,
                    NomeUsuario = "GCNT-CONTAS-ATUALIZACAO-CADASTRAL-CONSUMER",
                    MotivoBloqueio = 43,
                    SubMotivo = "Bloqueio Óbito GC - Contas Atualizacao Cadastral Consumer"
                }
            };
        }
    }
}
