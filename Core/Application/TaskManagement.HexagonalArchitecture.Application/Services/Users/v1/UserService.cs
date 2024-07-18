using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Application.Commom.ExtensionMethods.v1;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Users.v1
{
    public class UserService(UserManager<User> _userManager) : IUserService
    {
        public async Task<CustomResult<User>> GetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return CustomResult<User>.Success(user);
        }
        public async Task<CustomResult<User>> UpdateAsync(string email, string firstName, string lastName, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user is null)
                return CustomResult<User>.Failure(new CustomError("UserNotFound", "User not found."));

            user.Update(firstName, lastName, newEmail);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }
        public async Task<CustomResult<User>> CreateAsync(string firstName, string lastName, string email, string password)
        {
            var user = new User(firstName, lastName, email);

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }
    }
}
