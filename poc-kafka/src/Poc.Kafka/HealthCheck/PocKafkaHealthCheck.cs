using Poc.Kafka.Configs;
using Poc.Kafka.Managers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.HealthCheck;

[ExcludeFromCodeCoverage]
internal sealed class PocKafkaHealthCheck : IHealthCheck
{
    private readonly IList<string> _topicsToCheckInMetadata;
    private readonly PocKafkaAdminClientConfig _adminConfig;

    private PocKafkaHealthCheck(PocKafkaAdminClientConfig adminConfig, IList<string> topicsToCheck)
    {
        ArgumentNullException.ThrowIfNull(adminConfig);

        _topicsToCheckInMetadata = topicsToCheck;
        _adminConfig = adminConfig;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadata = ClusterManager.GetMetadata(_adminConfig);

            if (metadata.Brokers.Count == 0)
                return Task.FromResult(HealthCheckResult.Unhealthy("Broker unavailable."));

            var missingTopics = _topicsToCheckInMetadata
                .Except(metadata.Topics.Select(topicMetadata => topicMetadata.Topic), StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (missingTopics.Length != 0)
                return Task.FromResult(HealthCheckResult.Unhealthy($"Required topics not found: {string.Join(", ", missingTopics)}"));


            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Broker check failed: {ex.Message}"));
        }
    }

    internal static PocKafkaHealthCheck Create(PocKafkaAdminClientConfig adminConfig, IList<string> topicsToCheck) =>
        new(adminConfig, topicsToCheck);
}