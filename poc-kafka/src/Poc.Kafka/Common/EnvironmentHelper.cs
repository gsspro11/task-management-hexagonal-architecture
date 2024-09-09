using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common;

[ExcludeFromCodeCoverage]
internal static class EnvironmentHelper
{
    private const string NAME_ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
    private const string NAME_DOTNET_ENVIRONMENT = "DOTNET_ENVIRONMENT";

    internal static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable(NAME_ASPNETCORE_ENVIRONMENT)
               ?? Environment.GetEnvironmentVariable(NAME_DOTNET_ENVIRONMENT)
               ?? Environments.Staging;
    }

    internal static string GetMachineName() =>
        Environment.MachineName;
}
