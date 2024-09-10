using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;
using System.Collections;

namespace Poc.ContasAtualizacaoCadastralConsumer.Test.Shared.DataClasses
{
    public static class DataClass
    {
        public class BloquearContaCorrenteResponseData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return
                    GetTestData(StatusProcessamento.NaoDefinido);
                yield return
                    GetTestData(StatusProcessamento.ProcessadoSucesso);
                yield return
                    GetTestData(StatusProcessamento.InformacaoSolicitadaNaoEncontrada);
                yield return
                    GetTestData(StatusProcessamento.SemPermissaoAcesso);
                yield return
                    GetTestData(StatusProcessamento.ProcessadoComErro);
                yield return
                    GetTestData(StatusProcessamento.ContaAgenciaInformadaInvalida);
                yield return
                    GetTestData(StatusProcessamento.CartaoDeCreditoInvalido);
                yield return
                    GetTestData(StatusProcessamento.ProcessadoComExcecao);
                yield return
                    GetTestData(StatusProcessamento.ErroRegraDeNegocio);
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private static object[] GetTestData(StatusProcessamento statusProcessamento)
            {
                BloquearContaCorrenteResponse data = new()
                {
                    BloquearContaCorrenteResult = new()
                    {
                        StatusProcessamento = statusProcessamento
                    }
                };

                return [data];
            }
        }
    }
}
