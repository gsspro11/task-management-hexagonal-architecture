using Confluent.Kafka;

namespace Poc.Kafka.Common.Extensions;

internal static class DeliveryResportExtension
{
    private const string ErrorFormat = "Code: {0} - Reason: {1}";
    internal static string GetErrorFormatted(this Error error) =>
        string.Format(ErrorFormat, error.Code, error.Reason);
}
