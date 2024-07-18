using Microsoft.AspNetCore.Identity;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Application.Commom.ExtensionMethods.v1
{
    public static class IdentityResultExtensions
    {
        public static CustomResult<User> ErrorsTreatment(this IdentityResult result)
        {
            var errors = new List<CustomError>();

            foreach (var item in result.Errors)
                errors.Add(new CustomError(item.Code, item.Description));

            return CustomResult<User>.Failure([.. errors]);
        }
    }
}
