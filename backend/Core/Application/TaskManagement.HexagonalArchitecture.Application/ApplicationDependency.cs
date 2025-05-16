using TaskManagement.HexagonalArchitecture.Domain.Services.v1;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TaskManagement.HexagonalArchitecture.Application.Services.Assignments.v1;
using TaskManagement.HexagonalArchitecture.Application.Services.Comments.v1;
using TaskManagement.HexagonalArchitecture.Application.Services.Users.v1;

namespace TaskManagement.HexagonalArchitecture.Application
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationDependency
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
        }
    }
}
