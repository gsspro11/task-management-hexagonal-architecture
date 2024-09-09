using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Settings;

namespace Poc.Kafka.Configs.Validators;

internal static class PocKafkaConsumerConfigValidator
{
    internal static void Validate(PocKafkaConsumerConfig consumerConfig)
    {
        ValidateNullProperties(consumerConfig);
        ValidateTopics(consumerConfig);
        ValidateRetrySettings(consumerConfig);
        ValidateFetchSettings(consumerConfig);
        ValidatePartitionFetchBytes(consumerConfig);
        ValidateDelayPartitionEof(consumerConfig);
        PocKafkaCredentialsConfigValidator.Validate(consumerConfig);
    }

    private static void ValidateDelayPartitionEof(PocKafkaConsumerConfig consumerConfig)
    {
        if (consumerConfig.DelayPartitionEofMs < ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS)
            throw new ArgumentException($"When {nameof(consumerConfig.EnablePartitionEof)} is enabled, the value of {nameof(consumerConfig.DelayPartitionEofMs)} " +
                $"property must be greater than or equal to {ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS}. Current value: {consumerConfig.DelayPartitionEofMs}.");
    }
    private static void ValidatePartitionFetchBytes(PocKafkaConsumerConfig consumerConfig)
    {
        if (consumerConfig.MaxPartitionFetchBytes.HasValue &&
            (consumerConfig.MaxPartitionFetchBytes < ConsumerConstant.MIN_PARTITION_FETCH_BYTES || consumerConfig.MaxPartitionFetchBytes > ConsumerConstant.MAX_PARTITION_FETCH_BYTES))
            throw new ArgumentException($"The value of {nameof(consumerConfig.MaxPartitionFetchBytes)} must be between {ConsumerConstant.MIN_PARTITION_FETCH_BYTES} and {ConsumerConstant.MAX_PARTITION_FETCH_BYTES}. Current value: {consumerConfig.MaxPartitionFetchBytes}.");
    }
    private static void ValidateFetchSettings(PocKafkaConsumerConfig consumerConfig)
    {
        if (consumerConfig.FetchWaitMaxMs.HasValue && consumerConfig.FetchWaitMaxMs < 0)
            throw new ArgumentException($"The value of {nameof(consumerConfig.FetchWaitMaxMs)} must be non-negative.");

        if (consumerConfig.FetchMinBytes.HasValue && (consumerConfig.FetchMinBytes < ConsumerConstant.MIN_FETCH_MIN_BYTES || consumerConfig.FetchMinBytes > ConsumerConstant.MAX_FETCH_MIN_BYTES))
            throw new ArgumentException($"The value of {nameof(consumerConfig.FetchMinBytes)} must be between {ConsumerConstant.MIN_FETCH_MIN_BYTES} and {ConsumerConstant.MAX_FETCH_MIN_BYTES}. Current value: {consumerConfig.FetchMinBytes}.");

        if (consumerConfig.FetchMaxBytes.HasValue &&
            (consumerConfig.FetchMaxBytes < ConsumerConstant.MIN_FETCH_MAX_BYTES || consumerConfig.FetchMaxBytes > ConsumerConstant.MAX_FETCH_MAX_BYTES))
            throw new ArgumentException($"The value of {nameof(consumerConfig.FetchMaxBytes)} must be between {ConsumerConstant.MIN_FETCH_MAX_BYTES} and {ConsumerConstant.MAX_FETCH_MAX_BYTES}. Current value: {consumerConfig.FetchMaxBytes}.");
    }
    private static void ValidateTopics(PocKafkaConsumerConfig consumerConfig)
    {
        if (consumerConfig.Topics.Count <= 0)
            throw new ArgumentException($"No topics have been configured. Please specify at least one topic in the {nameof(consumerConfig.Topics)} property.");

        if (consumerConfig.TopicRetry is not null && consumerConfig.Topics.Exists(t => t.Topic == consumerConfig.TopicRetry))
            throw new ArgumentException($"{nameof(consumerConfig.TopicRetry)} must be unique and cannot be one of the subscribed topics.");

        if (consumerConfig.TopicDeadLetter is not null && (consumerConfig.Topics.Exists(t => t.Topic == consumerConfig.TopicDeadLetter) || consumerConfig.TopicDeadLetter == consumerConfig.TopicRetry))
            throw new ArgumentException($"{nameof(consumerConfig.TopicDeadLetter)} must be unique, cannot be one of the subscribed topics, and cannot be the same as {nameof(consumerConfig.TopicRetry)}.");

        ValidateUniqueTopicConfiguration(consumerConfig);
    }
    private static void ValidateNullProperties(PocKafkaConsumerConfig consumerConfig)
    {
        ArgumentNullException.ThrowIfNull(consumerConfig);
        ArgumentNullException.ThrowIfNull(consumerConfig.Topics);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerConfig.Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerConfig.BootstrapServers);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerConfig.GroupId);
    }
    private static void ValidateUniqueTopicConfiguration(PocKafkaConsumerConfig consumerConfig)
    {
        var allConfiguredTopics = consumerConfig.Topics.ToList();

        if (consumerConfig.TopicRetry is not null)
            allConfiguredTopics.Add(new PocTopicSettings { Topic = consumerConfig.TopicRetry });

        if (consumerConfig.TopicDeadLetter is not null)
            allConfiguredTopics.Add(new PocTopicSettings { Topic = consumerConfig.TopicDeadLetter });

        var duplicateTopics = allConfiguredTopics
            .GroupBy(x => x.Topic)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateTopics.Count != 0)
            throw new ArgumentException($"Duplicate topic configurations detected for: {string.Join(", ", duplicateTopics)}. Each topic must be unique.");
    }
    private static void ValidateRetrySettings(PocKafkaConsumerConfig consumerConfig)
    {
        if (consumerConfig.EnableRetryTopicConsumer)
        {
            if (string.IsNullOrWhiteSpace(consumerConfig.TopicRetry))
                throw new ArgumentException($"When {nameof(consumerConfig.EnableRetryTopicConsumer)} is enabled, the value of {nameof(consumerConfig.TopicRetry)} property is required.");

            if (consumerConfig.EnableAutoCommit)
                throw new ArgumentException($"When {nameof(consumerConfig.EnableRetryTopicConsumer)} is enabled, {nameof(consumerConfig.EnableAutoCommit)} must be false.");

            if (consumerConfig.RetryDelayMs < ConsumerConstant.MIN_RETRY_DELAY_MS || consumerConfig.RetryDelayMs > ConsumerConstant.MAX_RETRY_DELAY_MS)
                throw new ArgumentException($"When {nameof(consumerConfig.EnableRetryTopicConsumer)} is enabled, the retry delay must be within the specified range of {ConsumerConstant.MIN_RETRY_DELAY_MS} ms to {ConsumerConstant.MAX_RETRY_DELAY_MS} ms.");

            if (consumerConfig.RetryLimit < ConsumerConstant.MIN_RETRY_LIMIT || consumerConfig.RetryLimit > ConsumerConstant.MAX_RETRY_LIMIT)
                throw new ArgumentException($"When {nameof(consumerConfig.EnableRetryTopicConsumer)} is enabled, the retry limit must be within the specified range of {ConsumerConstant.MIN_RETRY_LIMIT} to {ConsumerConstant.MAX_RETRY_LIMIT}.");

            if (consumerConfig.MaxConcurrentMessages < ConsumerConstant.MIN_MAX_CONCURRENT_MESSAGES || consumerConfig.MaxConcurrentMessages > ConsumerConstant.MAX_MAX_CONCURRENT_MESSAGES_LIMIT)
                throw new ArgumentException($"When {nameof(consumerConfig.EnableRetryTopicConsumer)} is enabled, the maximum number of concurrent messages must be within the specified range of {ConsumerConstant.MIN_MAX_CONCURRENT_MESSAGES} to {ConsumerConstant.MAX_MAX_CONCURRENT_MESSAGES_LIMIT}.");
        }
    }
}
