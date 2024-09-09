using Poc.Api.Client.Extensions;
using Poc.Security.Factors;
using Poc.Security.Factors.Client;
using Poc.Security.Factors.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Poc.Factors.Extensions
{
    /// <summary>
    /// Extensão para o Service collection com os serviços do Factors
    /// Aplica o pattern descrito em https://learn.microsoft.com/en-us/dotnet/core/extensions/options-library-authors
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Configura e registra o POCFactors no service collection
        /// Opção para configuração utilizando Action
        /// </summary>
        /// <example>
        /// TODO
        /// </example>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddPocFactors(this IServiceCollection services,
                                                             Action<FactorsOptions> configureOptions)
        {
            return services.Configure(configureOptions).AddPocFactors();
        }

        /// <summary>
        /// Configura e registra o POCFactors no service collection
        /// Opção para configuração utilizando Section
        /// </summary>
        /// <example>
        /// services.AddPocFactors(builder.Configuration.GetSection("Factors"));
        /// </example>
        /// <param name="services"></param>
        /// <param name="configurationSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddPocFactors(this IServiceCollection services,
                                                             IConfiguration configurationSection)
        {
            return services
                .Configure<FactorsOptions>(configurationSection)
                .AddPocFactors();
        }

       

        /// <summary>
        /// Configura e registra o Factors no service collection
        /// </summary>
        /// <example>
        /// services.AddPocFactors();
        /// </example>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddPocFactors(this IServiceCollection services)
        {
            services.AddScoped<ISegurancaApiClient, SegurancaApiClient>();
            services.AddScoped<IFactors, Security.Factors.Factors>();
            services.AddPocApiClientService(1000);

            return services;
        }
    }
}
