using Microsoft.Extensions.DependencyInjection;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;

namespace TaskManagement.HexagonalArchitecture.Database.UnitOfWork.v1;

public class UnitOfWork(IServiceProvider provider, ApplicationDbContext dbContext) : IUnitOfWork
{
    public ICommentRepository Comments => provider.GetRequiredService<ICommentRepository>();
    public IAssignmentRepository Assignments => provider.GetRequiredService<IAssignmentRepository>();

    public Task SaveChanges(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}