using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Domain.Services.v1
{
    public interface IUserService
    {
        Task<CustomResult<User>> GetAsync(Guid id);
        Task<CustomResult<User>> DeleteAsync(string email);
        Task<CustomResult<User>> GetByEmailAsync(string email);
        Task<CustomResult<List<string>>> GetByUserNameAsync(string userName);
        Task<CustomResult<string>> LoginAsync(string email, string password);
        Task<CustomResult<User>> UpdateAsync(Guid id, string firstName, string lastName, string newEmail);
        Task<CustomResult<User>> CreateAsync(string firstName, string lastName, string email, string password);
    }
}
