using Poc.Kafka.Common.Constants;
using Poc.Kafka.Common.Settings;
using Confluent.Kafka;

namespace Poc.Kafka.Configs;

internal sealed class PocKafkaConsumerConfig : PocKafkaConfigBase, IPocKafkaConsumerConfig
{
    public string? Name { get; private set; }
    public List<PocTopicSettings> Topics { get; private set; } = [];
    public string? GroupId { get; private set; }
    public int? StatisticsIntervalMs { get; private set; }
    public int? SessionTimeoutMs { get; private set; }
    public int? HeartbeatIntervalMs { get; private set; }
    public int? MaxPollIntervalMs { get; private set; }
    public bool EnableAutoCommit { get; private set; } = false;
    public bool EnablePartitionEof { get; private set; } = true;
    public string? TopicRetry { get; private set; }
    public string? TopicDeadLetter { get; private set; }
    public bool EnableRetryTopicConsumer { get; private set; } = false;
    public int RetryLimit { get; private set; } = ConsumerConstant.RETRY_LIMIT_DEFAULT;
    public int RetryDelayMs { get; private set; } = ConsumerConstant.MIN_RETRY_DELAY_MS;
    public bool ApiVersionRequest { get; private set; } = true;
    public AutoOffsetReset? AutoOffsetReset { get; private set; }
    public int DelayPartitionEofMs { get; private set; } = ConsumerConstant.MIN_DELAY_PARTITION_EOF_MS;
    public int? FetchWaitMaxMs { get; private set; }
    public int? FetchMinBytes { get; private set; }
    public int? FetchMaxBytes { get; private set; }
    public int? MaxPartitionFetchBytes { get; private set; }
    public PartitionAssignmentStrategy? PartitionAssignmentStrategy { get; private set; }
    public int MaxConcurrentMessages { get; private set; } = ConsumerConstant.MIN_MAX_CONCURRENT_MESSAGES;
    internal bool IsConcurrentlyProcess => MaxConcurrentMessages > 1;

    public void SetName(string name) =>
        Name = name;

    public void SetTopic(PocTopicSettings topic)
    {
        Topics.Clear();
        Topics.Add(topic);
    }
    public void SetTopics(List<PocTopicSettings> topics) =>
        Topics = topics;

    public void SetGroupId(string groupId) =>
       GroupId = groupId;

    public void SetSessionTimeoutMs(int sessionTimeoutMs) =>
        SessionTimeoutMs = sessionTimeoutMs;

    public void SetHeartbeatIntervalMs(int heartbeatIntervalMs) =>
        HeartbeatIntervalMs = heartbeatIntervalMs;

    public void SetStatisticsIntervalMs(int statisticsIntervalMs) =>
        StatisticsIntervalMs = statisticsIntervalMs;

    public void SetMaxPollIntervalMs(int maxPollIntervalMs) =>
      MaxPollIntervalMs = maxPollIntervalMs;

    public void SetTopicRetry(string topicRetry) =>
        TopicRetry = topicRetry;

    public void SetTopicDeadLetter(string topicDeadLetter) =>
        TopicDeadLetter = topicDeadLetter;

    public void SetEnableRetryTopicConsumer() =>
        EnableRetryTopicConsumer = true;

    public void SetApiVersionRequest(bool apiVersionRequest) =>
        ApiVersionRequest = apiVersionRequest;

    public void SetAutoOffsetReset(AutoOffsetReset autoOffsetReset) =>
        AutoOffsetReset = autoOffsetReset;

    public void SetRetryLimit(int retryLimit) =>
        RetryLimit = retryLimit;

    public void SetRetryDelayMs(int retryDelayMs) =>
        RetryDelayMs = retryDelayMs;

    public void SetEnableAutoCommit() =>
        EnableAutoCommit = true;

    public void SetDelayPartitionEofMs(int delayPartitionEofMs) =>
        DelayPartitionEofMs = delayPartitionEofMs;

    public void SetFetchWaitMaxMs(int fetchWaitMaxMs) =>
        FetchWaitMaxMs = fetchWaitMaxMs;

    public void SetFetchMinBytes(int fetchMinBytes) =>
        FetchMinBytes = fetchMinBytes;

    public void SetFetchMaxBytes(int fetchMaxBytes) =>
        FetchMaxBytes = fetchMaxBytes;

    public void SetMaxPartitionFetchBytes(int maxPartitionFetchBytes) =>
        MaxPartitionFetchBytes = maxPartitionFetchBytes;

    public void SetPartitionAssignmentStrategy(PartitionAssignmentStrategy partitionAssignmentStrategy) =>
        PartitionAssignmentStrategy = partitionAssignmentStrategy;

    public void SetMaxConcurrentMessages(int maxConcurrentMessages) =>
       MaxConcurrentMessages = maxConcurrentMessages;
}
