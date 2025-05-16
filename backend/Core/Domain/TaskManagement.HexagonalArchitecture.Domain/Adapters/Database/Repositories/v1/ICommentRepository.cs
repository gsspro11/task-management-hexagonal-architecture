using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1
{
    public interface ICommentRepository : IRepository<Comment>
    {
        void Add(Comment comment);
        void Update(Comment comment);
        void Remove(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Comment>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Comment>> ListByAssignmentIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
