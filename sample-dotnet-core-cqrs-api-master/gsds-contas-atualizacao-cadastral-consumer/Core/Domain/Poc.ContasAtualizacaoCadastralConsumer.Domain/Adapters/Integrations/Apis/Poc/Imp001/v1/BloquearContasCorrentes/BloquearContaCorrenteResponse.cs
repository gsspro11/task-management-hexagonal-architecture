using System.Diagnostics;
using System.Xml.Serialization;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes
{
    [DebuggerStepThrough()]
    [XmlType(Namespace = "http://tempuri.org/")]
    public partial class BloquearContaCorrenteResponse
    {
        public BloquearContaCorrenteResult BloquearContaCorrenteResult { get; set; }
    }

    public partial class BloquearContaCorrenteResult
    {
        [XmlElement(Order = 0)]
        public string Observacao { get; set; }

        [XmlElement(Order = 1)]
        public StatusProcessamento StatusProcessamento { get; set; }

        [XmlElement(Order = 2)]
        public short IdentificadorErro { get; set; }

        [XmlElement(Order = 3)]
        public string DescricaoErro { get; set; }
    }

    [XmlType(Namespace = "http://tempuri.org/")]
    public enum StatusProcessamento
    {
        NaoDefinido,
        ProcessadoSucesso,
        InformacaoSolicitadaNaoEncontrada,
        SemPermissaoAcesso,
        ProcessadoComErro,
        ContaAgenciaInformadaInvalida,
        CartaoDeCreditoInvalido,
        ProcessadoComExcecao,
        ErroRegraDeNegocio,
    }
}
