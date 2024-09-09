﻿namespace Poc.RabbitMQ;

public static class Constants
{
    public const string HEADER_NAME_RETRY_COUNT = "RetryCount";

    public const int MIN_MESSAGE_SEND_RETRIES = 1;
    public const int MAX_MESSAGE_SEND_RETRIES = 10;

    public const int MAX_MESSAGE_SEND_RETRIES_WITH_ENABLE_IDEMPOTENCE = 3;

    public const int MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE = 1;
    public const int MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE = 5;

    public const int MIN_DELAY_PARTITION_EOF_MS_WITH_ENABLE_PARTITION_EOF = 500;

    public const int MAX_CONCURRENT_MESSAGES_LIMIT = 500;

    public const int MAX_CONSUME_RETRIES = 3;
    public const int DELAY_IN_SECONDS_BETWEEN_RETRIES = 2;

}
