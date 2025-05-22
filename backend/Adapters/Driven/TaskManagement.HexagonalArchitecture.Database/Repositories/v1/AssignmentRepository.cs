using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Database.Repositories.v1;

public class AssignmentRepository(ApplicationDbContext dbContext) : IAssignmentRepository
{
    public void Add(Assignment assignment)
    {
        dbContext.Set<Assignment>().Add(assignment);
    }

    public void Remove(Assignment assignment)
    {
        dbContext.Set<Assignment>().Remove(assignment);
    }

    public void Update(Assignment assignment)
    {
        dbContext.Set<Assignment>().Update(assignment);
    }

    public Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<Assignment>()
            .Where(g => g.Id.Equals(id))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Assignment>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<Assignment>()
            .Where(g => g.UserId.Equals(id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}