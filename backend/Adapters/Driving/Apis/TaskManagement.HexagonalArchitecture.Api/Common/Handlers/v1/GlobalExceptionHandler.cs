using Microsoft.AspNetCore.Diagnostics;
using TaskManagement.HexagonalArchitecture.Domain.Abstractions;

namespace TaskManagement.HexagonalArchitecture.Api.Common.Handlers.v1
{
    internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(
                exception, "Exception occurred: {Message}", exception.Message);

            var problemDetails = new CustomError[] { new(exception.GetType().Name, exception.Message) };

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
