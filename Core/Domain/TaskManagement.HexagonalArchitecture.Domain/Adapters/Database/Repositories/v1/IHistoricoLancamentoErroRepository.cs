using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1
{
    public interface IAssignmentRepository //: IGenericRepository<DatabaseConnection, long, HistoricoLancamentoErro>
    {
        Task<Assignment?> FindByTitleAsync(string email);
        Task<Assignment> UpdateAsync(Assignment assignment);
        Task<Assignment> CreateAsync(Assignment assignment);
    }
}
