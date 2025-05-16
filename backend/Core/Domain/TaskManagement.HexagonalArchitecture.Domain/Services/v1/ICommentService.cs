using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface ICommentService
    {
        Task<CustomResult<Comment>> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<CustomResult<Comment>> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<CustomResult<List<Comment>>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken);
        Task<CustomResult<List<Comment>>> ListByAssignmentIdAsync(Guid id, CancellationToken cancellationToken);

        Task<CustomResult<Comment>> UpdateAsync(Guid id, string description, CancellationToken cancellationToken);

        Task<CustomResult<Comment>> CreateAsync(string description, Guid assignmentId, Guid userId,
            CancellationToken cancellationToken);
    }
}