namespace WebApplication1.Model
{
    public class RequestPix
    {
        public int IdCartao {  get; set; }
        public int IdContaCartao { get; set; }
        public int ValorSaqueCartao { get; set; }
        string TipoPagamento { get; set; } = string.Empty;
        public decimal TaxaJurosMes { get; set; }
        public decimal TaxaIofDiario { get; set; }
        public decimal TaxaCetMes { get; set; }
        public decimal TaxaCetAno { get; set; }
        public decimal TotalDebitadoFatura { get; set; }
        public decimal LimiteDisponivel { get; set; }

        public RequestPixSeguranca Seguranca { get; set; }

        public string Descricao { get; set; } = String.Empty;

        public RequestPixFavorecido Favorecido { get; set; }

        public string FinalidadeTransacao { get; set; }
        public string IniciacaoFormulario { get; set; }
        public RequestPixPagador Pagador { get; set; }

        public string TipoInstrucao { get; set; }
        public decimal Valor { get; set; }
        public bool PagamentoPorQrCode { get; set; }
        public bool AcaoFavorecido { get; set; }
    }

    public class RequestPixPagador { 
        public string Agencia { get; set; }
        public int IdTipoConta { get; set; }
        public string Nome { get; set; }
        public string NumeroConta { get; set; }
    }

    public class RequestPixFavorecido {
        public string Agencia { get; set; }
        public RequestPixFavorecidoChave Chave { get; set; }

        public string IdentificacaoBanco { get; set; }
        public string Inscricao { get; set; }
        public string InscricaoMascarada { get; set; }
        public string Nome { get; set; }
        public string NomeComercial { get; set; }
        public string NomeInstituicao { get; set; }
        public string ? NumeroConta { get; set; }
        public string TipoConta { get; set; }
        public string TipoTitular { get; set; }
        public bool IsNotAgencyAccountKey { get; set; }
    }

    public class RequestPixFavorecidoChave
    {
        public string EndToEndId { get; set; }
        public string TipoChave { get; set; }
        public string Valor { get; set; }
        public string IdEndToEnd { get; set; }
        public string KeyStatus { get; set; }
        public string KeyType { get; set; }
        public string Value { get; set; }
        public int Stability { get; set; }
    }

    public class RequestPixSeguranca
    {
        public RequestPixSegurancaOtp Otp { get; set; }
        public string [] Fatores { get; set; }
    }

    public class RequestPixSegurancaOtp
    {
        public string Tipo { get; set; }
        public string Token { get; set; }
        public string DeviceId { get; set; }
    }
}
