using AutoFixture;
using Poc.ContasAtualizacaoCadastralConsumer.Application.Services.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Settings.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Test.Shared.DataClasses;
using Poc.ContasAtualizacaoCadastralConsumer.Application.Common.Extensions.v1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Poc.ContasAtualizacaoCadastralConsumer.Application.Test.Services.v1
{
    [ExcludeFromCodeCoverage]
    public class ContasAtualizacaoCadastralServiceTest
    {
        private static Imp001UrlSettings _imp001UrlSettings => new()
        {
            PathUrl = "https://poc.com.br/poc/wsIntegracaoMatera.asmx"
        };

        private static Imp001CredentialSettings _imp001CredentialSettings => new()
        {
            loginws = "INTEGR.EMCONTA",
            senhaws = "INTEGR.EMCONTA"
        };

        private ContasAtualizacaoCadastralMessage? _message;

        private readonly Mock<IGsdsApiManager> _gsdsApiManagerMock;
        private readonly Mock<IImp001ApiManager> _imp001ApiManagerMock;
        private readonly Mock<IOptions<Imp001UrlSettings>> _imp001UrlSettingsMock;
        private readonly Mock<IOptions<Imp001CredentialSettings>> _imp001CredentialSettingsMock;

        private readonly Mock<ILogger<ContasAtualizacaoCadastralService>> _loggerMock;

        public ContasAtualizacaoCadastralServiceTest()
        {
            _gsdsApiManagerMock = new Mock<IGsdsApiManager>();
            _imp001ApiManagerMock = new Mock<IImp001ApiManager>();
            _imp001UrlSettingsMock = new Mock<IOptions<Imp001UrlSettings>>();
            _imp001CredentialSettingsMock = new Mock<IOptions<Imp001CredentialSettings>>();

            _loggerMock = new Mock<ILogger<ContasAtualizacaoCadastralService>>();

            CommomSetup();
        }

        [Theory]
        [ClassData(typeof(DataClass.BloquearContaCorrenteResponseData))]
        public async Task DeveValidarProcessaAtualizacaoCadastralAsync(BloquearContaCorrenteResponse bloquearContaCorrenteResponse)
        {
            BuildMessage(ClienteSituacaoCadastral.CanceladaEncerramentoEspolio.GetDescription());

            _imp001ApiManagerMock
                .Setup(s => s.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>()))
                .ReturnsAsync(bloquearContaCorrenteResponse);

            var service = new ContasAtualizacaoCadastralService(
                _gsdsApiManagerMock.Object,
                _imp001ApiManagerMock.Object,
                _imp001UrlSettingsMock.Object,
                _imp001CredentialSettingsMock.Object,
                _loggerMock.Object
            );

            await service.ProcessarAtualizacaoCadastralAsync(_message!, It.IsAny<CancellationToken>());

            _gsdsApiManagerMock.Verify(x => x.ConsultarContasPessoaAsync(It.IsAny<string>()), Times.Once);
            _imp001ApiManagerMock.Verify(x => x.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>()), Times.Once);
        }

        [Theory]
        [InlineData(ClienteSituacaoCadastral.Suspensa, 0, 0)]
        [InlineData(ClienteSituacaoCadastral.CanceladaEncerramentoEspolio, 1, 1)]
        public async Task DeveValidarSituacaoCadastralProcessaAtualizacaoCadastralAsync(ClienteSituacaoCadastral situacaoCadastral, int qtdeExecucoesConsulta, int qtdeExecucoesBloqueio)
        {
            BuildMessage(situacaoCadastral.GetDescription());

            var service = new ContasAtualizacaoCadastralService(
                _gsdsApiManagerMock.Object,
                _imp001ApiManagerMock.Object,
                _imp001UrlSettingsMock.Object,
                _imp001CredentialSettingsMock.Object,
                _loggerMock.Object
            );

            await service.ProcessarAtualizacaoCadastralAsync(_message!, It.IsAny<CancellationToken>());

            _gsdsApiManagerMock.Verify(x => x.ConsultarContasPessoaAsync(It.IsAny<string>()), Times.Exactly(qtdeExecucoesConsulta));
            _imp001ApiManagerMock.Verify(x => x.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>()), Times.Exactly(qtdeExecucoesBloqueio));
        }

        private void CommomSetup()
        {
            var fixture = new Fixture()
                .Build<ConsultarContasPessoaResponse>()
                .With(x => x.ObterContasCorrentesPessoaResult, new ObterContasCorrentesPessoaResult
                {
                    Contas =
                    [
                        new Conta
                        {
                            DadosRetornaContaCorrente = new DadosRetornaContaCorrente
                            {
                                Situacao = "L"
                            }
                        }
                    ]
                })
                .Create();

            _imp001UrlSettingsMock.Setup(x => x.Value)
                .Returns(_imp001UrlSettings);

            _imp001CredentialSettingsMock.Setup(x => x.Value)
                .Returns(_imp001CredentialSettings);

            _gsdsApiManagerMock
                .Setup(s => s.ConsultarContasPessoaAsync(It.IsAny<string>()))
                .ReturnsAsync(fixture);
        }

        private void BuildMessage(string situacaoCadastral)
        {
            var schema = new Schema
            {
                Cliente = new Cliente
                {
                    Propriedades = new Propriedades
                    {
                        SituacaoCadastral = new SituacaoCadastral
                        {
                            Value = new Value
                            {
                                Name = situacaoCadastral
                            }
                        }
                    }
                }
            };

            _message = new Fixture().Build<ContasAtualizacaoCadastralMessage>()
                .With(x => x.Schema, schema)
                .Create();
        }
    }
}
