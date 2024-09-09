using Poc.RabbitMQ.Factories;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

namespace Poc.RabbitMQ.HealthCheck;

internal class PocRabbitMQHealthCheck : IHealthCheck
{
    private readonly RabbitMQConnectionResolver _rabbitMQConnectionResolver;

    public PocRabbitMQHealthCheck(RabbitMQConnectionResolver rabbitMQConnectionResolver)
    {
        _rabbitMQConnectionResolver = rabbitMQConnectionResolver;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionData = _rabbitMQConnectionResolver(context.Registration.Name.Split(" - ")[1]);

            var result = default(HealthCheckResult);

            if (connectionData.Connection.IsOpen)
                result = HealthCheckResult.Healthy();
            else
                result = HealthCheckResult.Unhealthy($"Broker unavailable. - {connectionData.Connection.CloseReason.ReplyText}");

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Failed to connect to broker. - {ex.Message}"));
        }
    }
}