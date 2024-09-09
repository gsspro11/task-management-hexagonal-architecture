using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Common;

[ExcludeFromCodeCoverage]
internal static class EnvironmentHelpers
{
    private const string NAME_ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
    private const string NAME_DOTNET_ENVIRONMENT = "DOTNET_ENVIRONMENT";

    public static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable(NAME_ASPNETCORE_ENVIRONMENT)
               ?? Environment.GetEnvironmentVariable(NAME_DOTNET_ENVIRONMENT)
               ?? "Staging";
    }
}
