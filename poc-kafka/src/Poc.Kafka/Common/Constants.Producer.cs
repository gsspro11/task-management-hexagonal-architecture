namespace Poc.Kafka;

internal static partial class Constants
{
    internal static class Producer
    {
        public const int MAX_MESSAGE_SEND_RETRIES_WITH_ENABLE_IDEMPOTENCE = 3;

        public const int MIN_MESSAGE_SEND_RETRIES = 1;
        public const int MAX_MESSAGE_SEND_RETRIES = 10;

        public const int MIN_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE = 1;
        public const int MAX_IN_FLIGHT_WITH_ENABLE_IDEMPOTENCE = 5;
    }
}
