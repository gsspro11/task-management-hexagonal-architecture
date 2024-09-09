using Poc.Security.Factors.Client;
using Poc.Security.Factors.Constants;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Poc.Security.Factors.Model;
using Poc.Security.Factors.Model.Response;
using Poc.Security.Factors.Model.Request;
using Microsoft.Extensions.Options;
using Poc.Security.Factors.Options;

namespace Poc.Security.Factors
{

    /// <summary>
    /// Componente responsavel pela verificacao de Fatores JWT
    /// </summary>
    public class Factors : IFactors
    {
        private readonly ISegurancaApiClient _apiClient;
        private readonly ILogger _logger;
        private readonly FactorsOptions _options;

        /// <summary>
        /// Construtor para IoC
        /// </summary>
        /// <param name="apiClient">SegurancaApiClient</param>
        /// <param name="logger">ILogger</param>
        /// <param name="options">Options</param>
        public Factors
        (
            ISegurancaApiClient apiClient,
            ILogger<IFactors> logger,
            IOptions<FactorsOptions> options
        )
        {
            _apiClient = apiClient;
            _logger = logger;
            _options = options.Value;
        }
            
        /// <summary>
        /// Valida a lista de fatores passados
        /// </summary>
        /// <param name="operacao">Operacao</param>
        /// <param name="fatores">Fatores a ser validados</param>
        /// <param name="cpfCliente">Cpf do cliente</param>
        /// <returns></returns>
        public async Task<bool> ValidateFactors(Operacao operacao, List<FatorOperacao> fatores, string cpfCliente)
        {
            var request = new ValidaFatoresOperacaoRequest()
            {
                Fatores = fatores,
                Valor = operacao
            };

            var url = _options.ApiBaseUrl + SegurancaApiConstants.ValidaFatoresOperacaoEndpoint;

            try
            {
                await _apiClient.ValidaFatoresOperacao(url, request, cpfCliente);
            }
            catch (FlurlHttpException ex)
            {
                var message = await ex.GetResponseStringAsync();
                _logger.LogError("Erro ao validar fatores. Statuscode: {statuscode}. Mensagem: {msg}", ex.StatusCode, message);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao validar fatores {msg}, {inner} ", ex.Message, ex.InnerException?.Message);
                return false;
            }

            return true;
        }
    }
}
