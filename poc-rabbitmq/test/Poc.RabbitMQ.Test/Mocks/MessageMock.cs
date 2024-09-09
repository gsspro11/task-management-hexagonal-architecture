using System.Diagnostics.CodeAnalysis;

namespace Poc.RabbitMQ.Test.Mocks;

[ExcludeFromCodeCoverage]
public class MessageMock
{
    public string Message { get; set; } = "Teste Message";
}
