using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Enums;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface IAssignmentService
    {
        Task<CustomResult<Assignment>> GetAsync(string title);
        Task<CustomResult<Assignment>> UpdateAsync(string title, string description, DateTime dueDate, int priority, AssignmentStatus status);
        Task<CustomResult<Assignment>> CreateAsync(string title, string description, DateTime dueDate, int priority, AssignmentStatus status);
    }
}
