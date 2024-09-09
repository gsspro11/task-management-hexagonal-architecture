using System.Diagnostics.CodeAnalysis;

namespace Poc.Kafka.Providers;

[ExcludeFromCodeCoverage]
internal sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
