using System.Diagnostics.CodeAnalysis;
namespace Poc.RabbitMQ.Configs;

[ExcludeFromCodeCoverage]
public static class PocRabbitMQValidatorConfig
{
    public static void ValidateConfig(PocRabbitMQConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(config?.HostName);

        ValidadePort(config.Port);
        ValidadeCredentials(config.IsCredentialsProvided, config.UserName, config.Password);
    }


    private static void ValidadePort(int port)
    {
        if (port <= 0)
        {
            throw new ArgumentException($"Port is invalid.");
        }
    }

    private static void ValidadeCredentials(bool isCredentialsProvided, string username, string password)
    {
        if (isCredentialsProvided && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
        {
            throw new ArgumentException($"Username and Password are required.");
        }
    }
}
