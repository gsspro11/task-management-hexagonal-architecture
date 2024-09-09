using Poc.Security.Factors.Client;
using Poc.Security.Factors.Model;
using Poc.Security.Factors.Model.Request;
using Microsoft.Extensions.Logging;
using Moq;
using Poc.Security.Factors.Constants;
using Flurl.Http;

namespace Poc.Security.Factors.Tests
{
    public class FactorsTests
    {

        [Fact]
        public async Task ValidateFactors_GivenCorrectFactors_WhenValidate_ExpectTrue()
        {
            //Arrange
            var clientMock = new Mock<ISegurancaApiClient>();
            var loggerMock = new Mock<ILogger<IFactors>>();
            var options = TestCommons.Options;
            var factors = new Factors(clientMock.Object,loggerMock.Object, options);
            var fatores = new List<FatorOperacao>();
            var operacao = Operacao.AtivacaoTokenOTP;
            var cpfCliente = TestCommons.Cpf;

            //Act
            var valido = await factors.ValidateFactors(operacao, fatores, cpfCliente);

            //Assert

            //confere chamada a api
            clientMock.Verify(c => c.ValidaFatoresOperacao(
                //confere se a url da api ta certa
                It.Is<string>(s => s == TestCommons.BaseUrl + SegurancaApiConstants.ValidaFatoresOperacaoEndpoint),
                //confere se os paremetros foram passados corretamente a api
                It.Is<ValidaFatoresOperacaoRequest>(r => r.Fatores == fatores && r.Valor == operacao),
                //confere se o cpf enviado a api é o mesmo passado
                It.Is<string>(c => c == TestCommons.Cpf)
            ), Times.Once());

            Assert.True(valido);
        }
        
        [Fact]
        public async Task ValidateFactors_GivenBlockedToken_WhenValidate_ExpectFalse()
        {
            //Arrange
            var clientMock = new Mock<ISegurancaApiClient>();
            var loggerMock = new Mock<ILogger<IFactors>>();

            var exception = new FlurlHttpException(null);

            clientMock
                .Setup(c => c.ValidaFatoresOperacao(It.IsAny<string>(), It.IsAny<ValidaFatoresOperacaoRequest>(), It.IsAny<string>()))
                .Throws(exception);

            var options = TestCommons.Options;
            var factors = new Factors(clientMock.Object, loggerMock.Object, options);
            var fatores = new List<FatorOperacao>();
            var operacao = Operacao.AtivacaoTokenOTP;
            var cpfCliente = TestCommons.Cpf;

            //Act
            var valido = await factors.ValidateFactors(operacao, fatores, cpfCliente);

            //Assert

            Assert.False(valido);
        }
    }

}