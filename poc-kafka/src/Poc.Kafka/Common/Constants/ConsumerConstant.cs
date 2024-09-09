namespace Poc.Kafka.Common.Constants;

public static class ConsumerConstant
{
    public const int MAX_ATTEMPTS_ON_ERROR_CONSUMPTION = 3;

    public const int DELAY_ON_ERROR_CONSUMPTION_MS = 2000;

    public const int DELAY_BETEWEEN_RETRY_ATTEMPTS_MS = 5000;

    public const int RETRY_LIMIT_DEFAULT = 3;
    public const int MIN_RETRY_LIMIT = 0;
    public const int MAX_RETRY_LIMIT = 10;

    public const int MIN_DELAY_PARTITION_EOF_MS = 100;

    public const int MIN_FETCH_MIN_BYTES = 1;
    public const int MAX_FETCH_MIN_BYTES = 1000000; //1MB

    public const int MIN_FETCH_MAX_BYTES = 65536;
    public const int MAX_FETCH_MAX_BYTES = 52428800;

    public const int MIN_PARTITION_FETCH_BYTES = 1;
    public const int MAX_PARTITION_FETCH_BYTES = 10485760; //10MB

    public const int MIN_RETRY_DELAY_MS = 0;
    public const int MAX_RETRY_DELAY_MS = 21600000; //6Hours

    public const int MIN_MAX_CONCURRENT_MESSAGES = 1;
    public const int MAX_MAX_CONCURRENT_MESSAGES_LIMIT = 500;


    public const string HEADER_NAME_RETRY_COUNT = "RetryCount";
    public const string HEADER_NAME_RETRY_AFTER = "RetryAffter";
}
