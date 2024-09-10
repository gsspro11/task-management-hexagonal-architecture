using System.ComponentModel;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums
{
    public enum ClienteSituacaoCadastral
    {
        [Description("SUSPENSA")]
        Suspensa = 2,

        [Description("CANCELADA_POR_OBITO_SEM_ESPOLIO")]
        CanceladaEncerramentoEspolio = 3
    }
}
