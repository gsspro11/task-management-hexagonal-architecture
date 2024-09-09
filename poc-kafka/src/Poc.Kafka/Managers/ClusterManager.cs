using Poc.Kafka.Configs;
using Poc.Kafka.Factories;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Managers;

[ExcludeFromCodeCoverage]
internal static class ClusterManager
{
    private static readonly TimeSpan MetadataTimeout = TimeSpan.FromSeconds(5);

    internal static void CreateTopicsIfNotExists(PocKafkaAdminClientConfig config, TopicConfiguration[] topics)
    {
        try
        {
            using var adminClient = KafkaAdminFactory.CreateAdminClient(config);
            var topicsToCreate = new List<TopicSpecification>();

            foreach (var topic in topics)
            {
                var metadata = adminClient.GetMetadata(topic.Name, MetadataTimeout);

                var topicMetadata = metadata.Topics.First(t => t.Topic == topic.Name);
                if (topicMetadata is null || topicMetadata.Error.IsError && topicMetadata.Error.Code == ErrorCode.UnknownTopicOrPart)
                {
                    topicsToCreate.Add(topic.MapToTopicSpecification());
                }
            }

            if (topicsToCreate.Count != 0)
            {
                adminClient.CreateTopicsAsync(topicsToCreate).GetAwaiter().GetResult();
            }

        }
        catch (CreateTopicsException ex)
        {
            bool hasSignificantError = !ex.Results.Exists(r => r.Error.Code != ErrorCode.TopicAlreadyExists && r.Error.IsError);
            if (hasSignificantError)
                throw;
        }
    }

    internal static Metadata GetMetadata(PocKafkaAdminClientConfig config)
    {
        using var adminClient = KafkaAdminFactory.CreateAdminClient(config);
        return adminClient.GetMetadata(MetadataTimeout);
    }
}