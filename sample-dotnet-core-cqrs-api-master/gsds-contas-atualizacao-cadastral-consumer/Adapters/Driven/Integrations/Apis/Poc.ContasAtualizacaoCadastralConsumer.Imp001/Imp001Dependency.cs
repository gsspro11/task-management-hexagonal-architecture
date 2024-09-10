using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Settings.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.ContasAtualizacaoCadastralConsumer.Imp001
{
    [ExcludeFromCodeCoverage]
    public static class Imp001Dependency
    {
        public static void AddImp001Module(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Imp001UrlSettings>(options => configuration.GetSection("Apis:Imp001").Bind(options));
            services.Configure<Imp001CredentialSettings>(options => configuration.GetSection("credentials:apis:imp001").Bind(options));
        }
    }
}
