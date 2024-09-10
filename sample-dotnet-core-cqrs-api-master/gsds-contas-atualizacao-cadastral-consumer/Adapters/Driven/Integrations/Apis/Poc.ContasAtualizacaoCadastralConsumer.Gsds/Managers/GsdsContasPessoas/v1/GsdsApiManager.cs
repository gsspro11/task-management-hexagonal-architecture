using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poc.Api.Client.Base;
using Poc.Api.Client;
using Poc.Api.Client.Extensions;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Settings.v1;
using Polly.Retry;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;
using Poc.ContasAtualizacaoCadastralConsumer.Shared.Configurations.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Gsds.Managers.GsdsContasPessoas.v1
{
    public class GsdsApiManager : ApiBase, IGsdsApiManager
    {
        private readonly GsdsUrlSettings _gsdsUrlSettings;
        private readonly AsyncRetryPolicy<IFlurlResponse> _asyncRetryPolicy;

        private readonly IGsdsAuthenticationManager _gsdsAuthenticationManager;
        private readonly ILogger<IGsdsApiManager> _logger;

        public GsdsApiManager(IPocApiClient apiClient, IGsdsAuthenticationManager gsdsAuthenticationManager,
            IOptions<GsdsUrlSettings> gsdsUrlSettings, ILogger<GsdsApiManager> logger)
            : base(gsdsUrlSettings.Value.PathUrl, apiClient, logger)
        {
            _gsdsUrlSettings = gsdsUrlSettings.Value;
            _asyncRetryPolicy = HttpRetryPolicy.GlobalRetryPolicy();

            _gsdsAuthenticationManager = gsdsAuthenticationManager;
            _logger = logger;
        }

        public async Task<ConsultarContasPessoaResponse?> ConsultarContasPessoaAsync(string cpfCnpj)
        {
            try
            {
                _gsdsAuthenticationManager.SetCancellationToken(CancellationToken);
                var autenticacaoResponse = await _gsdsAuthenticationManager.AutenticarGsdsContasPessoasAsync();

                var response = await _asyncRetryPolicy.ExecuteAsync(() => ApiClient
                    .Url(_gsdsUrlSettings.UrlObterContas)
                    .WithPocHeaders<ConsultarContasPessoaResponse>(HeaderType.Response)
                    .WithOAuthBearerToken(autenticacaoResponse.accessToken)
                    .AppendQueryParam("inscricao", cpfCnpj)
                    .GetAsync(HttpCompletionOption.ResponseHeadersRead, CancellationToken));

                var consultarContasPessoaResponse = await response.GetJsonAsync<ConsultarContasPessoaResponse>();

                _logger.LogInformation("Response consulta de contas no Gsds: {Response}.", args: JsonConvert.SerializeObject(consultarContasPessoaResponse));

                return consultarContasPessoaResponse;
            }
            catch (FlurlHttpException ex)
            {
                var responseString = await ex.GetResponseStringAsync();

                _logger.LogError(ex, "Erro ao realizar consulta de contas no Gsds.{ResponseString}",
                    args: !string.IsNullOrEmpty(responseString) ? $" Response Gsds: {responseString}." : default);

                throw;
            }
        }
    }
}