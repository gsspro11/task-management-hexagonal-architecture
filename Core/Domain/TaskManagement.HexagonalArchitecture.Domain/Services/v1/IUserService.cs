using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface IUserService
    {
        Task<CustomResult<User>> GetAsync(string email);
        Task<CustomResult<User>> UpdateAsync(string email, string firstName, string lastName, string newEmail);
        Task<CustomResult<User>> CreateAsync(string firstName, string lastName, string email, string password);
    }
}
