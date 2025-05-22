using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Application.Common.Authentication;
using TaskManagement.HexagonalArchitecture.Application.Common.ExtensionMethods.v1;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Users.v1
{
    public class UserService(
        UserManager<User> userManager,
        TokenProvider tokenProvider) : IUserService
    {
        public async Task<CustomResult<User>> GetAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return CustomResult<User>.Failure(new CustomError("UserNotFound", "User not found."));

            return CustomResult<User>.Success(user);
        }

        public async Task<CustomResult<User>> DeleteAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return CustomResult<User>.Failure(new CustomError("UserNotFound", "User not found."));

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }

        public async Task<CustomResult<User>> GetByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return CustomResult<User>.Failure(new CustomError("UserNotFound", "User not found."));

            return CustomResult<User>.Success(user);
        }

        public async Task<CustomResult<List<string>>> GetByUserNameAsync(string userName)
        {
            var users = userManager.Users;

            var filteredUsers = users
                .Where(u => u.UserName.ToLower().Contains(userName.ToLower()))
                .Select(x => x.UserName)
                .ToList();

            return await Task.FromResult(CustomResult<List<string>>.Success(filteredUsers));
        }

        public async Task<CustomResult<string>> LoginAsync(string email, string password)
        {
            var resultGet = await GetByEmailAsync(email);

            if (resultGet.IsFailure)
                return CustomResult<string>.Failure(resultGet.Error!);

            var user = resultGet.Value;

            var validPassword = await userManager.CheckPasswordAsync(user, password);

            if (!validPassword)
                return CustomResult<string>.Failure(new CustomError("InvalidPassword", "Invalid password."));

            return CustomResult<string>.Success(tokenProvider.Create(user));
        }

        public async Task<CustomResult<User>> UpdateAsync(Guid id, string firstName, string lastName,
            string newEmail)
        {
            var resultGet = await GetAsync(id);

            if (resultGet.IsFailure)
                return resultGet;

            var user = resultGet.Value;

            user.Update(firstName, lastName, newEmail);

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }

        public async Task<CustomResult<User>> CreateAsync(string firstName, string lastName, string email,
            string password)
        {
            var user = new User(firstName, lastName, email);

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return result.ErrorsTreatment();

            return CustomResult<User>.Success(user);
        }
    }
}