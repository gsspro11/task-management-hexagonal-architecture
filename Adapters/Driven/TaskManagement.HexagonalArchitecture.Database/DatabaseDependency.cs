using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace TaskManagement.HexagonalArchitecture.Database
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseDependency
    {
        public static void AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<Domain.Adapters.Database.UnitOfWork.v1.IUnitOfWork, UnitOfWork.v1.UnitOfWork>();

            //services.AddConnectionManager<DatabaseConnection>()
            //    .AddConnection<Domain.Adapters.Database.UnitOfWork.v1.IUnitOfWorkContext, UnitOfWork.v1.UnitOfWorkContext>
            //    (
            //        DatabaseConnection.Oracle,
            //        (provider, options) =>
            //        {
            //            options.UseOracle(configuration.GetConnectionString("ExadataConnection"));
            //        }
            //    );

            //services.AddScopedRepository<IHistoricoLancamentoErroRepository, HistoricoLancamentoErroRepository>();
        }
    }
}
