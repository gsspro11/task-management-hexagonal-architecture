using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Enums;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Assignments.v1
{
    public class AssignmentService(IUnitOfWork unitOfWork, IUserService userService)
        : IAssignmentService
    {
        public async Task<CustomResult<Assignment>> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var assignment = await unitOfWork.Assignments.GetByIdAsync(id, cancellationToken);

            if (assignment is null)
                return CustomResult<Assignment>.Failure(new CustomError("AssignmentNotFound", "Assignment not found."));

            return CustomResult<Assignment>.Success(assignment);
        }

        public async Task<CustomResult<Assignment>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var assignment = await unitOfWork.Assignments.GetByIdAsync(id, cancellationToken);

            if (assignment is null)
                return CustomResult<Assignment>.Failure(new CustomError("AssignmentNotFound", "Assignment not found."));

            unitOfWork.Assignments.Remove(assignment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Assignment>.Success(assignment);
        }
        public async Task<CustomResult<List<Assignment>>> ListByUserIdAsync(Guid id,
            CancellationToken cancellationToken)
        {
            var assignments = await unitOfWork.Assignments.ListByUserIdAsync(id, cancellationToken);

            if (assignments is null or { Count: 0 })
                return CustomResult<List<Assignment>>.Failure(new CustomError("AssignmentsNotFound",
                    "Assignments not found."));

            return CustomResult<List<Assignment>>.Success(assignments);
        }

        public async Task<CustomResult<Assignment>> UpdateAsync(Guid id, string title, string description,
            DateTime dueDate, int priority, AssignmentStatus status, CancellationToken cancellationToken)
        {
            var resultGet = await GetAsync(id, cancellationToken);

            if (resultGet.IsFailure)
                return resultGet;

            var assignment = resultGet.Value;

            assignment.Update(title, description, dueDate, priority, status);

            unitOfWork.Assignments.Update(assignment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Assignment>.Success(assignment);
        }

        public async Task<CustomResult<Assignment>> CreateAsync(string title, string description, string userName,
            DateTime dueDate, int priority, AssignmentStatus status, CancellationToken cancellationToken)
        {
            var resultGet = await userService.GetByEmailAsync(userName);

            var user = resultGet.Value;

            var assignment = new Assignment(Guid.NewGuid(), title, description, user.Id, dueDate, priority, status);

            unitOfWork.Assignments.Add(assignment);

            await unitOfWork.SaveChanges(cancellationToken);

            return CustomResult<Assignment>.Success(assignment);
        }
    }
}