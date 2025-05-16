namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1
{
    public interface IUnitOfWorkContext
    {
        //DatabaseFacade GetDatabase();

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// TBL_WTH
        /// </summary>
        //DbSet<HistoricoLancamento> HistoricosLancamentos { get; }
        //DbSet<SolicitacaoLancamento> SolicitacoesLancamentos { get; }
        //DbSet<HistoricoLancamentoErro> HistoricosLancamentosErros { get; }
        //DbSet<SolicitacaoLancamentoProcessamento> SolicitacoesLancamentosProcessamentos { get; }
    }
}
