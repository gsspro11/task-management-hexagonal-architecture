using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface IUserService
    {
        Task<CustomResult<User>> Register(string firstName, string lastName, string email, string password);
    }
}
