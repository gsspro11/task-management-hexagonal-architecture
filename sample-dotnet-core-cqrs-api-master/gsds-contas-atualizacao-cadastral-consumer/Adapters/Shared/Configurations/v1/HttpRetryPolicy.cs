using Flurl.Http;
using Polly;
using Polly.Retry;
using System.Net;

namespace Poc.ContasAtualizacaoCadastralConsumer.Shared.Configurations.v1
{
    public static class HttpRetryPolicy
    {
        const int retryCount = 2;
        const int retryAttempt = 2;

        public static AsyncRetryPolicy<IFlurlResponse> GlobalRetryPolicy()
        {
            return Policy
                .Handle<HttpRequestException>()
                .Or<FlurlHttpTimeoutException>()
                .Or<FlurlHttpException>(r => { return RetryOnResult(r.StatusCode); })
                .OrResult<IFlurlResponse>(r => { return RetryOnResult(r.StatusCode); })

                .WaitAndRetryAsync(retryCount, provider => TimeSpan.FromSeconds(retryAttempt),
                    (ex, timeSpan) =>
                    {
                        Console.WriteLine($"Retrying...Error {ex}");
                    });
        }

        private static bool RetryOnResult(int? statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout.GetHashCode() ||
                   statusCode >= HttpStatusCode.InternalServerError.GetHashCode();
        }
    }
}
