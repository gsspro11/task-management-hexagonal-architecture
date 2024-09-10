using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1;
using Polly.Retry;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;
using Poc.Api.Client.Base;
using Poc.Api.Client;
using Poc.Api.Client.Extensions;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Settings.v1;
using Microsoft.Extensions.Options;
using Poc.ContasAtualizacaoCadastralConsumer.Shared.Configurations.v1;
using Flurl.Http;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Extensions.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Imp001.Managers.v1
{
    public class Imp001ApiManager : ApiBase, IImp001ApiManager
    {
        private readonly AsyncRetryPolicy<IFlurlResponse> _asyncRetryPolicy;

        private readonly ILogger<Imp001ApiManager> _logger;

        public Imp001ApiManager(IPocApiClient apiClient, IOptions<Imp001UrlSettings> imp001UrlSettings,
            ILogger<Imp001ApiManager> logger) : base(imp001UrlSettings.Value.PathUrl, apiClient, logger)
        {
            _asyncRetryPolicy = HttpRetryPolicy.GlobalRetryPolicy();

            _logger = logger;
        }

        public async Task<BloquearContaCorrenteResponse> BloquearContaCorrenteAsync(BloquearContaCorrente request)
        {
            try
            {
                var response = await _asyncRetryPolicy.ExecuteAsync(() => ApiClient
                    .Url()
                    .WithPocHeaders<BloquearContaCorrenteResponse>(HeaderType.Response)
                    .SoapRequestAsync(request, completionOption: HttpCompletionOption.ResponseHeadersRead, cancellationToken: CancellationToken));

                var bloquearContaCorrenteResponse = (await response.GetStringAsync())
                    .Deserialize<Envelope>()?.Body?.BloquearContaCorrenteResponse;

                _logger.LogInformation("Response bloquear conta corrente Imp001: {Response}.", args: JsonConvert.SerializeObject(bloquearContaCorrenteResponse));

                return bloquearContaCorrenteResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao bloquear conta corrente Imp001. Request: {request}.{response}", JsonConvert.SerializeObject(request),
                    !string.IsNullOrEmpty(ex.Message) ? $" Response Imp001: {ex.Message}." : default);

                return new BloquearContaCorrenteResponse
                {
                    BloquearContaCorrenteResult = new BloquearContaCorrenteResult
                    {
                        DescricaoErro = ex.Message,
                        StatusProcessamento = StatusProcessamento.ProcessadoComExcecao
                    }
                };
            }
        }
    }
}
