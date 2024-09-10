using AutoFixture;
using Poc.Api.Client;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.Autenticacao;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.ConsultarContasPessoas;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Managers.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Settings.v1;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Poc.ContasAtualizacaoCadastral.Gsds.Test.v1
{
    [ExcludeFromCodeCoverage]
    public class GsdsApiManagerTest
    {
        private static GsdsUrlSettings _gsdsUrlSettings => new()
        {
            PathUrl = "https://gsds-contas-pessoas-consultas-hml.cloudpoc.app.br",
            UrlObterContas = "v1/pessoas",
        };

        private readonly Mock<IOptions<GsdsUrlSettings>> _gsdsUrlSettingsMock;

        private readonly Mock<IPocApiClient> _pocApiClientMock;
        private readonly Mock<ILogger<GsdsApiManager>> _pocLoggingMock;
        private readonly Mock<IGsdsAuthenticationManager> _gsdsAuthenticationManagerMock;

        public GsdsApiManagerTest()
        {
            _gsdsUrlSettingsMock = new Mock<IOptions<GsdsUrlSettings>>();

            _pocApiClientMock = new Mock<IPocApiClient>();
            _pocLoggingMock = new Mock<ILogger<GsdsApiManager>>();
            _gsdsAuthenticationManagerMock = new Mock<IGsdsAuthenticationManager>();

            CommonSetup();
        }

        [Fact]
        public async Task DeveValidarTipoRetornoConsultarContasPessoaAsync()
        {
            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlObterContas))
                .Returns(_gsdsUrlSettings.PathUrl.WithSettings(config => { }));

            using var httpTest = new HttpTest();

            httpTest
                .RespondWithJson(
                    new Fixture().Build<ConsultarContasPessoaResponse>().Create(),
                    (int)HttpStatusCode.Created);

            var manager = new GsdsApiManager(
                _pocApiClientMock.Object,
                _gsdsAuthenticationManagerMock.Object,
                _gsdsUrlSettingsMock.Object,
                _pocLoggingMock.Object
            );

            //Act
            var result = await manager.ConsultarContasPessoaAsync(It.IsAny<string>());

            //ShouldBe
            result.ShouldBeOfType<ConsultarContasPessoaResponse>();
            result.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task DeveValidarTratamentoExcecaoConsultarContasPessoaAsync(HttpStatusCode httpStatusCode)
        {
            const string message = "Erro ao realizar consulta de contas no Gsds.";

            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlObterContas))
                .Throws(CreateFlurlHttpException(httpStatusCode));

            using var httpTest = new HttpTest();

            httpTest
                .RespondWithJson(
                    new Fixture().Build<ConsultarContasPessoaResponse>().Create(),
                    (int)httpStatusCode);

            var manager = new GsdsApiManager(
                _pocApiClientMock.Object,
                _gsdsAuthenticationManagerMock.Object,
                _gsdsUrlSettingsMock.Object,
                _pocLoggingMock.Object
            );

            try
            {
                //Act
                await manager.ConsultarContasPessoaAsync(It.IsAny<string>());
            }
            catch (FlurlHttpException ex)
            {
                //Assert
                Assert.Equal((HttpStatusCode)ex.StatusCode!, httpStatusCode);

                _pocLoggingMock.Verify(logger => logger.Log(
                      It.IsAny<LogLevel>()
                    , It.IsAny<EventId>()
                    , It.Is<It.IsAnyType>((object v, Type _) => v.ToString()!.Contains(message))
                    , It.IsAny<Exception>()
                    , It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            }
        }

        public static FlurlHttpException CreateFlurlHttpException(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(string.Empty)
            };

            return new FlurlHttpException
            (new Fixture().Build<FlurlCall>()
                .OmitAutoProperties()
                .With(a => a.Request, new FlurlRequest { Url = _gsdsUrlSettings.PathUrl + _gsdsUrlSettings.UrlObterContas })
                .With(a => a.HttpRequestMessage, new HttpRequestMessage())
                .With(a => a.HttpResponseMessage, response)
                .With(a => a.Response, new FlurlResponse(new FlurlCall { HttpResponseMessage = response }))
                .Create());
        }

        private void CommonSetup()
        {
            _gsdsUrlSettingsMock.Setup(x => x.Value)
                .Returns(_gsdsUrlSettings);

            _gsdsAuthenticationManagerMock.Setup(x => x.AutenticarGsdsContasPessoasAsync())
                .ReturnsAsync(new Fixture().Build<AutenticacaoResponse>().Create());
        }
    }
}
