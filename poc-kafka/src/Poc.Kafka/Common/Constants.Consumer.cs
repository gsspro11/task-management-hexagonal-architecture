namespace Poc.Kafka;

internal static partial class Constants
{
    internal static class Consumer
    {
        public const int MAX_CONCURRENT_MESSAGES_LIMIT = 100;

        public const int MAX_FETCH_MIN_BYTES_LIMIT = 1000000; // 1MB

        public const int MIN_FETCH_MAX_BYTES = 65536;
        public const int MAX_FETCH_MAX_BYTES = 52428800;

        public const int MIN_PARTITION_FETCH_BYTES = 1;
        public const int MAX_PARTITION_FETCH_BYTES = 10485760; //10MB

        public const int MIN_DELAY_PARTITION_EOF_MS_WITH_ENABLE_PARTITION_EOF = 100;

        public const string HEADER_NAME_RETRY_COUNT = "RetryCount";

        public const int DELAY_IN_SECONDS_BETWEEN_RETRIES = 2;
        public const int MAX_RETRY_DELAY_MS = 60000;
    }
}
