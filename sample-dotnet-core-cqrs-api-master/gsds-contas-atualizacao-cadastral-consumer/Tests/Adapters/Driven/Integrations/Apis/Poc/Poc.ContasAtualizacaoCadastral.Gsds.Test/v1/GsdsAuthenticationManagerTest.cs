using AutoFixture;
using Poc.Api.Client;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1.Autenticacao;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Managers.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Settings.v1;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Poc.ContasAtualizacaoCadastral.Gsds.Test.v1
{
    [ExcludeFromCodeCoverage]
    public class GsdsAuthenticationManagerTest
    {
        const string cacheKey = "AUTENTICACAO-CACHE";
        const string exceptionMessage = "Erro ao autenticar Gsds.";

        private static GsdsUrlSettings _gsdsUrlSettings => new()
        {
            PathUrl = "https://gsds-contas-pessoas-consultas-hml.cloudpoc.app.br",
            UrlAutenticacao = "v1/authenticate"
        };
        private static GsdsCredentialSettings _gsdsCredentialSettings => new()
        {
            username = "Username",
            password = "Password"
        };

        private readonly Mock<IOptions<GsdsUrlSettings>> _gsdsUrlSettingsMock;
        private readonly Mock<IOptions<GsdsCredentialSettings>> _gsdsCredentialSettingsMock;

        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IPocApiClient> _pocApiClientMock;
        private readonly Mock<ILogger<GsdsApiManager>> _pocLoggingMock;

        public GsdsAuthenticationManagerTest()
        {
            _gsdsUrlSettingsMock = new Mock<IOptions<GsdsUrlSettings>>();
            _gsdsCredentialSettingsMock = new Mock<IOptions<GsdsCredentialSettings>>();

            _memoryCacheMock = new Mock<IMemoryCache>();
            _pocApiClientMock = new Mock<IPocApiClient>();
            _pocLoggingMock = new Mock<ILogger<GsdsApiManager>>();

            CommonSetup();
        }

        [Fact]
        public async Task DeveValidarTipoRetornoAutenticarGsdsContasPessoasAsync()
        {
            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlAutenticacao))
                .Returns(_gsdsUrlSettings.PathUrl.WithSettings(config => { }));

            using var httpTest = new HttpTest();

            httpTest
                .RespondWithJson(
                    new Fixture().Build<AutenticacaoResponse>().Create(),
                    (int)HttpStatusCode.Created);

            var manager = new GsdsAuthenticationManager(
                _pocApiClientMock.Object,
                _gsdsUrlSettingsMock.Object,
                _gsdsCredentialSettingsMock.Object,
                _memoryCacheMock.Object,
                _pocLoggingMock.Object
            );

            //Act
            var result = await manager.AutenticarGsdsContasPessoasAsync();

            //ShouldBe
            result.ShouldBeOfType<AutenticacaoResponse>();
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeveValidarTipoRetornoAutenticarGsdsContasPessoasCacheAsync()
        {
            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlAutenticacao))
                .Returns(_gsdsUrlSettings.PathUrl.WithSettings(config => { }));

            var cache = new MemoryCache(new MemoryCacheOptions());

            cache.Set(cacheKey,
                new Fixture().Build<AutenticacaoResponse>().Create(),
                TimeSpan.FromSeconds(5));

            var manager = new GsdsAuthenticationManager(
                _pocApiClientMock.Object,
                _gsdsUrlSettingsMock.Object,
                _gsdsCredentialSettingsMock.Object,
                cache,
                _pocLoggingMock.Object
            );

            //Act
            var result = await manager.AutenticarGsdsContasPessoasAsync();

            //ShouldBe
            result.ShouldBeOfType<AutenticacaoResponse>();
            result.ShouldNotBeNull();
        }

        [Fact]
        public async Task DeveValidarTratamentoExceptionAutenticarGsdsContasPessoasAsync()
        {
            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlAutenticacao))
                .Throws(new Exception());

            using var httpTest = new HttpTest();

            httpTest
                .RespondWithJson(
                    new Fixture().Build<AutenticacaoResponse>().Create());

            var manager = new GsdsAuthenticationManager(
                _pocApiClientMock.Object,
                _gsdsUrlSettingsMock.Object,
                _gsdsCredentialSettingsMock.Object,
                _memoryCacheMock.Object,
                _pocLoggingMock.Object
            );

            try
            {
                //Act
                await manager.AutenticarGsdsContasPessoasAsync();
            }
            catch (Exception)
            {
                //Verify
                _pocLoggingMock.Verify(logger => logger.Log(
                      It.IsAny<LogLevel>()
                    , It.IsAny<EventId>()
                    , It.Is<It.IsAnyType>((object v, Type _) => v.ToString()!.Contains(exceptionMessage))
                    , It.IsAny<Exception>()
                    , It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task DeveValidarTratamentoFlurlHttpExceptionAutenticarGsdsContasPessoasAsync(HttpStatusCode httpStatusCode)
        {
            _pocApiClientMock.Setup(data => data.Url(_gsdsUrlSettings.UrlAutenticacao))
                .Throws(CreateFlurlHttpException(httpStatusCode));

            using var httpTest = new HttpTest();

            httpTest
                .RespondWithJson(
                    new Fixture().Build<AutenticacaoResponse>().Create(),
                    (int)httpStatusCode);

            var manager = new GsdsAuthenticationManager(
                _pocApiClientMock.Object,
                _gsdsUrlSettingsMock.Object,
                _gsdsCredentialSettingsMock.Object,
                _memoryCacheMock.Object,
                _pocLoggingMock.Object
            );

            try
            {
                //Act
                await manager.AutenticarGsdsContasPessoasAsync();
            }
            catch (FlurlHttpException ex)
            {
                //Assert
                Assert.Equal((HttpStatusCode)ex.StatusCode!, httpStatusCode);

                _pocLoggingMock.Verify(logger => logger.Log(
                      It.IsAny<LogLevel>()
                    , It.IsAny<EventId>()
                    , It.Is<It.IsAnyType>((object v, Type _) => v.ToString()!.Contains(exceptionMessage))
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

            _gsdsCredentialSettingsMock.Setup(x => x.Value)
                .Returns(_gsdsCredentialSettings);

            _memoryCacheMock.Setup(x => x.CreateEntry(cacheKey))
                .Returns(Mock.Of<ICacheEntry>);
        }
    }
}
