using AutoFixture;
using Poc.Api.Client;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1.BloquearContasCorrentes;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Extensions.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Managers.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Settings.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Test.Shared.DataClasses;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Poc.ContasAtualizacaoCadastral.Imp001.Test.v1
{
    [ExcludeFromCodeCoverage]
    public class Imp001ApiManagerTest
    {
        private static Imp001UrlSettings _imp001UrlSettings => new()
        {
            PathUrl = "https://poc.com.br/poc/wsIntegracaoMatera.asmx"
        };

        private readonly Mock<IOptions<Imp001UrlSettings>> _imp001UrlSettingsMock;

        private readonly Mock<IPocApiClient> _pocApiClientMock;
        private readonly Mock<ILogger<Imp001ApiManager>> _pocLoggingMock;

        public Imp001ApiManagerTest()
        {
            _imp001UrlSettingsMock = new Mock<IOptions<Imp001UrlSettings>>();

            _pocApiClientMock = new Mock<IPocApiClient>();
            _pocLoggingMock = new Mock<ILogger<Imp001ApiManager>>();

            CommomSetup();
        }

        [Fact]
        public async Task DeveValidarTipoRetornoBloquearContaCorrenteAsync()
        {
            var xmlResponse = CreateXmlResponse(new Fixture().Build<BloquearContaCorrenteResponse>().Create());

            using var httpTest = new HttpTest();

            httpTest
                .RespondWith(xmlResponse, (int)HttpStatusCode.OK);

            var manager = new Imp001ApiManager(_pocApiClientMock.Object,
                _imp001UrlSettingsMock.Object,
                _pocLoggingMock.Object
            );

            //Act
            var result = await manager.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>());

            //ShouldBe
            result.ShouldBeOfType<BloquearContaCorrenteResponse>();
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeveValidarTratamentoExcecaoBloquearContaCorrenteAsync()
        {
            const string message = "Erro IMP001";

            var manager = new Imp001ApiManager(_pocApiClientMock.Object,
                _imp001UrlSettingsMock.Object,
                _pocLoggingMock.Object
            );

            //Act
            try
            {
                await manager.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>());
            }
            catch (Exception ex)
            {
                //Assert
                Assert.Equal(message, ex.Message);
                _pocLoggingMock.Verify(logger => logger.Log(
                      It.IsAny<LogLevel>()
                    , It.IsAny<EventId>()
                    , It.Is<It.IsAnyType>((object v, Type _) => v.ToString()!.Contains(message))
                    , It.IsAny<Exception>()
                    , It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            }
        }

        [Theory]
        [ClassData(typeof(DataClass.BloquearContaCorrenteResponseData))]
        public async Task DeveValidarStatusProcessamentoBloquearContaCorrenteAsync(BloquearContaCorrenteResponse bloquearContaCorrenteResponse)
        {
            var xmlResponse = CreateXmlResponse(bloquearContaCorrenteResponse);

            using var httpTest = new HttpTest();
            httpTest
                .RespondWith(xmlResponse, (int)HttpStatusCode.OK);

            var manager = new Imp001ApiManager(_pocApiClientMock.Object,
                 _imp001UrlSettingsMock.Object,
                 _pocLoggingMock.Object
             );

            //Act
            var result = await manager.BloquearContaCorrenteAsync(It.IsAny<BloquearContaCorrente>());

            //ShouldBe
            result.ShouldNotBeNull();
            result.BloquearContaCorrenteResult.StatusProcessamento.ShouldBe(bloquearContaCorrenteResponse.BloquearContaCorrenteResult.StatusProcessamento);
        }

        private void CommomSetup()
        {
            _imp001UrlSettingsMock.Setup(x => x.Value)
                            .Returns(_imp001UrlSettings);

            _pocApiClientMock.Setup(data => data.Url())
                .Returns(_imp001UrlSettings.PathUrl.WithSettings(config => { }));
        }
        private static string CreateXmlResponse(BloquearContaCorrenteResponse bloquearContaCorrenteResponse)
        {
            var envelope = new Envelope
            {
                Body = new EnvelopeBody
                {
                    BloquearContaCorrenteResponse = bloquearContaCorrenteResponse
                }
            };

            return envelope.Serialize();
        }
    }
}
