using TaskManagement.HexagonalArchitecture.Domain.Primitives;

namespace TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;

public interface IRepository<T>
    where T : AggregateRoot
{
}