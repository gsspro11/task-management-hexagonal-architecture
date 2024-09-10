using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Settings.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Poc.ContasAtualizacaoCadastralConsumer.Gsds.Managers.GsdsContasPessoas.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Gsds.GsdsContasPessoas.v1;

namespace Poc.ContasAtualizacaoCadastralConsumer.Gsds
{
    [ExcludeFromCodeCoverage]
    public static class GsdsDependency
    {
        public static void AddGsdsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GsdsUrlSettings>(options => configuration.GetSection("Apis:GsdsContasPessoas").Bind(options));
            services.Configure<GsdsCredentialSettings>(options => configuration.GetSection("credentials:apis:gsdscontaspessoas").Bind(options));
            services.AddScoped<IGsdsApiManager, GsdsApiManager>();
            services.AddScoped<IGsdsAuthenticationManager, GsdsAuthenticationManager>();
        }
    }
}
