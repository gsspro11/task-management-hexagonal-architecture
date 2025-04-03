using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Database.Repositories.v1;

public class AssignmentRepository : IAssignmentRepository
{
    public Task<Assignment?> FindByTitleAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Assignment> UpdateAsync(Assignment assignment)
    {
        throw new NotImplementedException();
    }

    public Task<Assignment> CreateAsync(Assignment assignment)
    {
        throw new NotImplementedException();
    }
}