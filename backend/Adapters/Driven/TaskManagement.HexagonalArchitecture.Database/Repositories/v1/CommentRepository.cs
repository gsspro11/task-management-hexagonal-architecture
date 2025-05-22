using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Database.Repositories.v1;

public class CommentRepository(ApplicationDbContext dbContext) : ICommentRepository
{
    public void Add(Comment comment)
    {
        dbContext.Set<Comment>().Add(comment);
    }

    public void Remove(Comment comment)
    {
        dbContext.Set<Comment>().Remove(comment);
    }

    public void Update(Comment comment)
    {
        dbContext.Set<Comment>().Update(comment);
    }

    public Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<Comment>()
            .Where(g => g.Id.Equals(id))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Comment>> ListByUserIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<Comment>()
            .Where(g => g.UserId.Equals(id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<List<Comment>> ListByAssignmentIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Set<Comment>()
            .Where(c => c.AssignmentId.Equals(id))
            .OrderBy(c => c.CreatedDate)
            .Include(c => c.User)
            .Select(c => new Comment(c.AssignmentId, c.Description, c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : null))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}