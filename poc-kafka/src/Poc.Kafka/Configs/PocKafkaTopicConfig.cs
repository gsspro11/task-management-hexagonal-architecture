using Poc.Kafka.Common;
using Confluent.Kafka.Admin;
using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Configs;


[ExcludeFromCodeCoverage]
internal record TopicConfiguration(
    string Name,
    int NumberOfPartitions,
    short ReplicationFactor,
    RetentionPeriodMs TimeToRetainDataMs = RetentionPeriodMs.NoRetention)
{
    public TopicSpecification MapToTopicSpecification()
    {
        var topicSpecification = new TopicSpecification
        {
            Name = Name,
            NumPartitions = NumberOfPartitions,
            ReplicationFactor = ReplicationFactor,

        };

        if (TimeToRetainDataMs != RetentionPeriodMs.NoRetention)
        {
            topicSpecification.Configs.Add("retention.ms", ((long)TimeToRetainDataMs).ToString());
        }

        return topicSpecification;
    }
}