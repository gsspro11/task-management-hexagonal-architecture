using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common.Extensions;

[ExcludeFromCodeCoverage]
internal static class ServiceCollectionExtensions
{
    internal static bool IsServiceRegistered<T>(this IServiceCollection services, string serviceKey)
    {
        return services.Any(descriptor =>
            descriptor.IsKeyedService &&
            descriptor.ServiceType == typeof(T) &&
            descriptor.ServiceKey as string == serviceKey);
    }
}