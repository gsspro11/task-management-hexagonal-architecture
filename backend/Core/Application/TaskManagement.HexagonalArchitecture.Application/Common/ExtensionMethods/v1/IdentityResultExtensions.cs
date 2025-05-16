using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Common.ExtensionMethods.v1
{
    public static class IdentityResultExtensions
    {
        public static CustomResult<User> ErrorsTreatment(this IdentityResult result)
        {
            var errors = result.Errors.Select(item => new CustomError(item.Code, item.Description)).ToList();

            return CustomResult<User>.Failure([.. errors]);
        }
    }
}
