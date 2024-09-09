namespace Poc.Kafka.Test.CommonTests;

internal static class TestConstant
{
    public const string TOPIC_FAKE = "topic";
    public const string TOPIC_RETRY_FAKE = "topic-retry";
    public const string TOPIC_DLQ_FAKE = "topic-dlq";

    public const string VALUE_FAKE = "value";
    public const string KEY_FAKE = "key";

    public const string EXPECTED_ERROR_REASON_PARTIAL_MESSAGE = "Code: Local_Partial - Reason: Partial message was received.";
}
