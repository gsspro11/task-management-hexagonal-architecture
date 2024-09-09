using Poc.Security.Factors.Model.Request;

namespace Poc.Security.Factors.Client
{
    public interface ISegurancaApiClient
    {
        Task FluxoAutenticacao(string heartbeat, string enderecoIp, string riskdata, string origem, string cpfcliente);
        Task ValidaFatoresOperacao(string url, ValidaFatoresOperacaoRequest request, string cpfCliente);
    }
}