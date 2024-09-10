using Poc.ContasAtualizacaoCadastralConsumer.Domain.Services.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Settings.v1;
using Microsoft.Extensions.Options;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums;
using Poc.ContasAtualizacaoCadastralConsumer.Application.Common.Extensions.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Extensions.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Application.Services.v1
{
    public class ContasAtualizacaoCadastralService : IContasAtualizacaoCadastralService
    {
        private const string SituacaoContaDesativada = "D";

        private readonly Imp001UrlSettings _imp001UrlSettings;
        private readonly Imp001CredentialSettings _imp001CredentialSettings;

        private readonly IGsdsApiManager _gsdsApiManager;
        private readonly IImp001ApiManager _imp001ApiManager;

        private readonly ILogger<ContasAtualizacaoCadastralService> _logger;

        public ContasAtualizacaoCadastralService
        (
            IGsdsApiManager gsdsApiManager,
            IImp001ApiManager imp001ApiManager,
            IOptions<Imp001UrlSettings> imp001UrlSettings,
            IOptions<Imp001CredentialSettings> imp001CredentialSettings,
            ILogger<ContasAtualizacaoCadastralService> logger
        )
        {
            _gsdsApiManager = gsdsApiManager;
            _imp001ApiManager = imp001ApiManager;

            _logger = logger;

            _imp001UrlSettings = imp001UrlSettings.Value;
            _imp001CredentialSettings = imp001CredentialSettings.Value;
        }

        public async Task ProcessarAtualizacaoCadastralAsync(ContasAtualizacaoCadastralMessage message, CancellationToken cancellationToken)
        {
            try
            {
                if (!ClienteSituacaoCadastral.CanceladaEncerramentoEspolio.GetDescription()
                    .Equals(message.Schema?.Cliente?.Propriedades?.SituacaoCadastral?.Value?.Name))
                    return;

                SetCancellationTokens(cancellationToken);

                var contasPessoaResponse = await _gsdsApiManager.ConsultarContasPessoaAsync(message.CpfCnpj);

                if (contasPessoaResponse?.ObterContasCorrentesPessoaResult?.Contas?.Length > 0)
                    await BloquearContasCorrentesAsync(contasPessoaResponse.ObterContasCorrentesPessoaResult.Contas);
            }
            catch (Exception ex)
            {
                LogContasAtualizacaoCadastralServiceError(message, ex);

                throw;
            }
        }

        private void SetCancellationTokens(CancellationToken cancellationToken)
        {
            _gsdsApiManager.SetCancellationToken(cancellationToken);
            _imp001ApiManager.SetCancellationToken(cancellationToken);
        }
        private async Task<BloquearContaCorrenteResponse[]> BloquearContasCorrentesAsync(Conta[] contas)
        {
            var results = new List<BloquearContaCorrenteResponse>();

            foreach (var item in contas.Where(x => x?.DadosRetornaContaCorrente is not null
                        && !x.DadosRetornaContaCorrente.Situacao.Equals(SituacaoContaDesativada)))
            {
                var request = item.BuildBloquearContaCorrenteRequest(_imp001CredentialSettings.loginws,
                    _imp001CredentialSettings.senhaws,
                    _imp001UrlSettings.PathUrl);

                var response = await _imp001ApiManager.BloquearContaCorrenteAsync(request);

                results.Add(response);
            }

            return [.. results];
        }
        private void LogContasAtualizacaoCadastralServiceError(ContasAtualizacaoCadastralMessage message, Exception ex)
        {
            _logger.LogError(ex, "Exceção ContasAtualizacaoCadastralServiceAsync: {message}. Mensagem: {value}.",
                                    ex.Message, JsonConvert.SerializeObject(message));
        }
    }
}
