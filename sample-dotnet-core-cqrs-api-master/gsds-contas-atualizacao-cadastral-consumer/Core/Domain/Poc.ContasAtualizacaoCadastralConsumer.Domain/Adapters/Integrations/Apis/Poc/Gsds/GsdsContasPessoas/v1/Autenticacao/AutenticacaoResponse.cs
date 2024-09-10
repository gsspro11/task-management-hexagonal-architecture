namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.Autenticacao
{
    public class AutenticacaoResponse
    {
        public string accessToken { get; set; }
        public int expiresIn { get; set; }
        public string idToken { get; set; }
        public string tokenType { get; set; }
    }
}
