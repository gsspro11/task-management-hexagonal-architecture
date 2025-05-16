using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TaskManagement.HexagonalArchitecture.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.Repositories.v1;
using TaskManagement.HexagonalArchitecture.Domain.Adapters.Database.UnitOfWork.v1;
using TaskManagement.HexagonalArchitecture.Domain.Entities.v1;

namespace TaskManagement.HexagonalArchitecture.Database
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseDependency
    {
        public static void AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServerConnection") ??
                                   throw new InvalidOperationException(
                                       "Connection string 'SqlServerConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork.v1.UnitOfWork>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        }
    }
}
