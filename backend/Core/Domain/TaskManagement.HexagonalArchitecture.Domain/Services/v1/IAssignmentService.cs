using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Enums;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface IAssignmentService
    {
        Task<CustomResult<Assignment>> GetAsync(Guid id, CancellationToken cancellationToken);
        
        Task<CustomResult<Assignment>> DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task<CustomResult<List<Assignment>>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken);
        
        Task<CustomResult<Assignment>> UpdateAsync(Guid id, string title, string description, DateTime dueDate,
            int priority,
            AssignmentStatus status, CancellationToken cancellationToken);

        Task<CustomResult<Assignment>> CreateAsync(string title, string description, string userName, DateTime dueDate,
            int priority, AssignmentStatus status, CancellationToken cancellationToken);
    }
}