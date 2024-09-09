using Microsoft.AspNetCore.Http;

namespace Poc.Security.Factors.Constants
{
    public static class SegurancaApiConstants
    {
        public const string ValidaFatoresOperacaoEndpoint = "validaFatoresOperacao";

        public const int StatusCodeTokenBloqueado = StatusCodes.Status422UnprocessableEntity;

        public const int StatusCodeValoresInvalidos = StatusCodes.Status400BadRequest;

    }
}
