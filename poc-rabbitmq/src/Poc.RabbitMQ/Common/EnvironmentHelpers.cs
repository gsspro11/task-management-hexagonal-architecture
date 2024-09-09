using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.RabbitMQ.Common
{
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
}
