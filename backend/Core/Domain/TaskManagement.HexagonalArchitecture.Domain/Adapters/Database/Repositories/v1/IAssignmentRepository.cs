using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        void Add(Assignment assignment);
        void Update(Assignment assignment);
        void Remove(Assignment assignment);
        Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Assignment>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
