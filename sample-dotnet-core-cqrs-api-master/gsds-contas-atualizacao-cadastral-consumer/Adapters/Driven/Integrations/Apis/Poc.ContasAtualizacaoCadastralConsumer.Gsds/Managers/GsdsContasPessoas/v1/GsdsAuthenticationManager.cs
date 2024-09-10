using Microsoft.Extensions.Logging;
using Poc.Api.Client.Base;
using Poc.Api.Client;
using Poc.Api.Client.Extensions;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Settings.v1;
using Polly.Retry;
using Microsoft.Extensions.Caching.Memory;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.Autenticacao;
using Poc.ContasAtualizacaoCadastralConsumer.Shared.Configurations.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Gsds.Managers.GsdsContasPessoas.v1
{
    public class GsdsAuthenticationManager : ApiBase, IGsdsAuthenticationManager
    {
        const string cacheKey = "AUTENTICACAO-CACHE";

        private readonly IMemoryCache _memoryCache;
        private readonly AsyncRetryPolicy<IFlurlResponse> _asyncRetryPolicy;

        private readonly GsdsUrlSettings _gsdsUrlSettings;
        private readonly GsdsCredentialSettings _gsdsCredentialSettings;

        private readonly ILogger<IGsdsApiManager> _logger;

        public GsdsAuthenticationManager(IPocApiClient apiClient, IOptions<GsdsUrlSettings> urlOptions,
            IOptions<GsdsCredentialSettings> credentialOptions, IMemoryCache memoryCache, ILogger<GsdsApiManager> logger)
            : base(urlOptions.Value.PathUrl, apiClient, logger)
        {
            _asyncRetryPolicy = HttpRetryPolicy.GlobalRetryPolicy();

            _logger = logger;
            _memoryCache = memoryCache;

            _gsdsUrlSettings = urlOptions.Value;
            _gsdsCredentialSettings = credentialOptions.Value;
        }

        public async Task<AutenticacaoResponse?> AutenticarGsdsContasPessoasAsync()
        {
            try
            {
                var autenticacao = await _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
                {
                    var response = await _asyncRetryPolicy.ExecuteAsync(() => ApiClient
                        .Url(_gsdsUrlSettings.UrlAutenticacao)
                        .WithPocHeaders<AutenticacaoResponse>(HeaderType.Response)
                        .WithBasicAuth(_gsdsCredentialSettings.username, _gsdsCredentialSettings.password)
                        .PostAsync(default, HttpCompletionOption.ResponseHeadersRead, CancellationToken));

                    var autenticacaoResponse = await response.GetJsonAsync<AutenticacaoResponse>();

                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(autenticacaoResponse.expiresIn));

                    return autenticacaoResponse;
                });

                _logger.LogInformation("Autenticação no Gsds realizada com sucesso.");

                return autenticacao;
            }
            catch (FlurlHttpException ex)
            {
                var responseString = await ex.GetResponseStringAsync();

                _logger.LogError(ex, "Erro ao autenticar Gsds.{ResponseString}",
                    args: !string.IsNullOrEmpty(responseString) ? $" Response autenticação Gsds: {responseString}." : default);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar Gsds.{ResponseString}",
                    args: !string.IsNullOrEmpty(ex.Message) ? $" Response autenticação Gsds: {ex.Message}." : default);

                throw;
            }
        }
    }
}
