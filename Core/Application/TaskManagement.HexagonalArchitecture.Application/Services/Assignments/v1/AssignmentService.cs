using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Enums;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Assignments.v1
{
    public class AssignmentService(IAssignmentRepository assignmentRepository) : IAssignmentService
    {
        public async Task<CustomResult<Assignment>> GetAsync(string email)
        {
            var assignment = await assignmentRepository.FindByTitleAsync(email);

            if (assignment is null)
                return CustomResult<Assignment>.Failure(new CustomError("AssignmentNotFound", "Assignment not found."));

            return CustomResult<Assignment>.Success(null);
        }
        public async Task<CustomResult<Assignment>> UpdateAsync(string title, string description, DateTime dueDate, int priority, AssignmentStatus status)
        {
            var resultGet = await GetAsync(title);

            if (resultGet.IsFailure)
                return resultGet;

            var assignment = resultGet.Value;

            assignment.Update(title, description, dueDate, priority, status);

            var result = await assignmentRepository.UpdateAsync(assignment);

            return CustomResult<Assignment>.Success(result);
        }
        public async Task<CustomResult<Assignment>> CreateAsync(string title, string description, DateTime dueDate, int priority, AssignmentStatus status)
        {
            var assignment = new Assignment(title, description, dueDate, priority, status);

            var result = await assignmentRepository.CreateAsync(assignment);

            return CustomResult<Assignment>.Success(result);
        }
    }
}
