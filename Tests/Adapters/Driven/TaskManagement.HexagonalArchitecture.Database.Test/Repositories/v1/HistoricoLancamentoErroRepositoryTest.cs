using System.Diagnostics.CodeAnalysis;

namespace TaskManagement.HexagonalArchitecture.Database.Test.Repositories.v1
{
    [ExcludeFromCodeCoverage]
    public class HistoricoLancamentoErroRepositoryTest
    {
        //private UnitOfWork.v1.UnitOfWork? _UnitOfWork;

        //private readonly Mock<IServiceProvider> _serviceProviderMock;
        //private readonly Mock<IHistoricoLancamentoErroRepository> _historicoLancamentoErroRepositoryMock;
        //public HistoricoLancamentoErroRepositoryTest()
        //{
        //    _serviceProviderMock = new Mock<IServiceProvider>();
        //    _historicoLancamentoErroRepositoryMock = new Mock<IHistoricoLancamentoErroRepository>();

        //    CommomSetup();
        //}

        //[Fact]
        //public async Task DeveValidarIncluirHistoricoErroLancamentoAsync()
        //{
        //    await _UnitOfWork!.HistoricosLancamentosErros.InsertAsync(It.IsAny<HistoricoLancamentoErro>());

        //    _historicoLancamentoErroRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<HistoricoLancamentoErro>()), Times.Once);
        //}

        //private void CommomSetup()
        //{
        //    _historicoLancamentoErroRepositoryMock.Setup(data => data.InsertAsync(It.IsAny<HistoricoLancamentoErro>()))
        //        .ReturnsAsync(It.IsAny<long>());

        //    _serviceProviderMock.Setup(data => data.GetService(typeof(IHistoricoLancamentoErroRepository)))
        //        .Returns(_historicoLancamentoErroRepositoryMock.Object);

        //    _UnitOfWork = new UnitOfWork.v1.UnitOfWork(_serviceProviderMock.Object);
        //}
    }
}
