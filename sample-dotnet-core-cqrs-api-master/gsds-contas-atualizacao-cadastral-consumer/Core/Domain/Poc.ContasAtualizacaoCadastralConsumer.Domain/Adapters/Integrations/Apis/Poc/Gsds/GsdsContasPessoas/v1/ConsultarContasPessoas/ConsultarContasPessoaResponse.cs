namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas
{
    public class ConsultarContasPessoaResponse
    {
        public ObterContasCorrentesPessoaResult ObterContasCorrentesPessoaResult { get; set; }
    }

    public class ObterContasCorrentesPessoaResult
    {
        public string NomeTitular { get; set; }
        public string NomeReduzido { get; set; }
        public int IdPessoa { get; set; }
        public Conta[] Contas { get; set; }
    }

    public class Conta
    {
        public DadosRetornaContaCorrente DadosRetornaContaCorrente { get; set; }
    }

    public class DadosRetornaContaCorrente
    {
        public int Agencia { get; set; }
        public string NomeAgencia { get; set; }
        public int NumeroConta { get; set; }
        public int TipoConta { get; set; }
        public string DescricaoTipoConta { get; set; }
        public string Situacao { get; set; }
        public string DescricaoSituacao { get; set; }
        public DateTime Abertura { get; set; }
        public CategoriaConta CategoriaConta { get; set; }
    }

    public class CategoriaConta
    {
        public string Id { get; set; }
        public string Descricao { get; set; }
    }

}
