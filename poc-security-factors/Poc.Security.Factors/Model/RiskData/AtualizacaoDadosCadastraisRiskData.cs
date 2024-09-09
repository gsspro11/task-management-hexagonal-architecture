namespace Poc.Security.Factors.Model.RiskData
{
    public  class AtualizacaoDadosCadastraisRiskData: IRiskData
    {
        public AtualizacaoDadosCadastraisRiskDataEndereco Endereço { get; set; }

        public string subOperacao { get; set; }
    }

    public class AtualizacaoDadosCadastraisRiskDataEndereco
    { 
        public string Cidade { get; set; }
        public string Numero { get; set; }
        public string Uf { get; set; }
        public string Bairro { get; set; }
        public bool SemNumero { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Complemento { get; set; }
    }
}
