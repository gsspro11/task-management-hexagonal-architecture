using System.Diagnostics;
using System.Xml.Serialization;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes
{
    [DebuggerStepThrough()]
    [XmlType(Namespace = "http://tempuri.org/")]
    public partial class BloquearContaCorrente
    {
        public Parametros parametros { get; set; }
    }

    public partial class Parametros
    {
        [XmlElement(Order = 0)]
        public string SenhaWs { get; set; }

        [XmlElement(Order = 1)]
        public string LoginWs { get; set; }

        [XmlElement(Order = 2)]
        public string UrlWs { get; set; }

        [XmlElement(Order = 3)]
        public int IdOrigemIntegracao { get; set; }

        [XmlElement(Order = 4)]
        public short Agencia { get; set; }

        [XmlElement(Order = 5)]
        public int NumeroConta { get; set; }

        [XmlElement(Order = 6)]
        public short MotivoBloqueio { get; set; }

        [XmlElement(Order = 7)]
        public string NomeUsuario { get; set; }

        [XmlElement(Order = 8)]
        public string SubMotivo { get; set; }
    }
}
