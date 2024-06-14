using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;
using TaskManagement.HexagonalArchitecture.Domain.Services.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Services.Users.v1
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CustomResult<User>> Register(string firstName, string lastName, string email, string password)
        {
            var user = new User(firstName, lastName, email);

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = new List<CustomError>();

                foreach (var item in result.Errors)
                    errors.Add(new CustomError(item.Code, item.Description));

                return CustomResult<User>.Failure([.. errors]);
            }

            return CustomResult<User>.Success(user);
        }
    }
}
