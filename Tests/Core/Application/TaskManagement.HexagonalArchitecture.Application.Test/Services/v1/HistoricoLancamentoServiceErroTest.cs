using System.Diagnostics.CodeAnalysis;

namespace TaskManagement.HexagonalArchitecture.Application.Test.Services.v1
{
    [ExcludeFromCodeCoverage]
    public class HistoricoLancamentoErroServiceTest
    {
        //private ProcessaLancamentoCcMessage? message;

        //private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        //public HistoricoLancamentoErroServiceTest()
        //{
        //    _unitOfWorkMock = new Mock<IUnitOfWork>();

        //    CommomSetup();
        //}

        //[Fact]
        //public async Task DeveValidarIncluirHistoricoLancamentoErroAsync()
        //{
        //    _unitOfWorkMock
        //       .Setup(data => data.HistoricosLancamentosErros.InsertAsync(It.IsAny<HistoricoLancamentoErro>()))
        //       .ReturnsAsync(1);

        //    var service = new HistoricoLancamentoErroService(
        //        _unitOfWorkMock.Object,
        //    );

        //    await service.IncluirHistoricoLancamentoErroAsync(message!, It.IsAny<RetornoProcessamento>());

        //    _unitOfWorkMock.Verify(x => x.HistoricosLancamentosErros.InsertAsync(It.IsAny<HistoricoLancamentoErro>()), Times.Once);
        //}
        //[Fact]
        //public async Task DeveValidarExcecaoIncluirHistoricoLancamentoErroAsync()
        //{
        //    _unitOfWorkMock
        //       .Setup(data => data.HistoricosLancamentosErros.InsertAsync(It.IsAny<HistoricoLancamentoErro>()))
        //       .ReturnsAsync(0);

        //    var service = new HistoricoLancamentoErroService(
        //        _unitOfWorkMock.Object
        //    );

        //    try
        //    {
        //        await service.IncluirHistoricoLancamentoErroAsync(message!, It.IsAny<RetornoProcessamento>());
        //    }
        //    catch (TaskManagementException ex)
        //    {
        //        Assert.Equal(RetornoProcessamento.ERRO_INCLUIR_HISTORICO_LANCAMENTO_ERRO, ex.RetornoProcessamento);
        //    }
        //}

        //private void CommomSetup()
        //{
        //    var fixture = new Fixture();
        //    fixture.Customizations.Add(new DateOnlySpecimenBuilder());

        //    message = new Fixture().Build<ProcessaLancamentoCcMessage>()
        //        .With(w => w.SolicitacaoLancamentoId, It.IsAny<long>())
        //        .With(w => w.LancamentoUuid, It.IsAny<Guid>().ToString())
        //        .Create();
        //}
    }
}
