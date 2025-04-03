using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Application.Common.ExtensionMethods.v1;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Users.v1
{
    public class UserService(UserManager<User> userManager) : IUserService
    {
        public async Task<CustomResult<User>> GetAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return CustomResult<User>.Failure(new CustomError("UserNotFound", "User not found."));

            return CustomResult<User>.Success(user);
        }
        public async Task<CustomResult<User>> UpdateAsync(string email, string firstName, string lastName, string newEmail)
        {
            var resultGet = await GetAsync(email);

            if (resultGet.IsFailure)
                return resultGet;

            var user = resultGet.Value;

            user.Update(firstName, lastName, newEmail);

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }
        public async Task<CustomResult<User>> CreateAsync(string firstName, string lastName, string email, string password)
        {
            var user = new User(firstName, lastName, email);

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }
    }
}
