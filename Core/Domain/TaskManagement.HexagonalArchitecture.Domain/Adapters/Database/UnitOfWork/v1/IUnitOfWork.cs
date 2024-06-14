using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// TBL_WTH
        /// </summary>
        IHistoricoLancamentoErroRepository HistoricosLancamentosErros { get; }
    }
}
