using Poc.Kafka.Common.Extensions;
using Confluent.Kafka;
namespace Poc.Kafka.Test.Common.Extensions;

public class DeliveryResportExtensionTest
{

    [Fact]
    public void GetErrorFormatted_ReturnsFormattedString()
    {
        var error = new Error(ErrorCode.Local_Partial, "Partial message was received");
        var expectedFormattedError = "Code: Local_Partial - Reason: Partial message was received";

        var formattedError = error.GetErrorFormatted();

        Assert.Equal(expectedFormattedError, formattedError);
    }
}
