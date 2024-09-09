using Poc.Api.Client;
using Poc.Security.Factors.Model.Request;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Poc.Security.Factors.Client
{
    public class SegurancaApiClient : ISegurancaApiClient
    {
        private readonly IPocApiClient _apiClient;

        public SegurancaApiClient
        (
            IPocApiClient apiClient
        )
        {
            _apiClient = apiClient;

            FlurlHttp.Configure(settings =>
                {
                    var jsonSettings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                    settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
                }
            );
        }

        public async Task FluxoAutenticacao(string heartbeat, string enderecoIp, string riskdata, string origem, string cpfcliente) 
        {

        }

        public async Task ValidaFatoresOperacao(string url, ValidaFatoresOperacaoRequest request, string cpfCliente)
        {
            await _apiClient
                .Url(url)
                .WithHeader("cpfcliente", cpfCliente)
                .PostJsonAsync(request)
                .ReceiveJson();
        }

    }
}
