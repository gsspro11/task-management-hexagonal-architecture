using Poc.ContasAtualizacaoCadastralConsumer.Application.Services.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Apis.Poc.Imp001.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Domain.Services.v1;
using Poc.ContasAtualizacaoCadastralConsumer.Imp001.Managers.v1;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.ContasAtualizacaoCadastralConsumer.Application
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationDependency
    {
        public static void AddApplicationModule(this IServiceCollection services)
        {
            services.AddScoped<IImp001ApiManager, Imp001ApiManager>();
            services.AddScoped<IContasAtualizacaoCadastralService, ContasAtualizacaoCadastralService>();
        }
    }
}
