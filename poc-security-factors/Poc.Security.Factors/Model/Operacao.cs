namespace Poc.Security.Factors.Model
{
    /// <summary>
    ///  Valores de operacao permitidos na API
    /// </summary>
    public enum Operacao
    {
        AtivacaoTokenOTPDatablink,
        AtivacaoTokenOTP, 
        AtivacaoOTPPoc, 
        EsqueciMinhaSenha, 
        DesbloqueioConta, 
        AtualizacaoDadosPessoais, 
        CreditoPessoal, 
        SaqueDigital, 
        SaqueComplementar, 
        Pix, 
        PagamentoConta, 
        RecargaCelular, 
        PrimeiroAcesso, 
        PrimeiroAcessoPJAtacado
    }
}
