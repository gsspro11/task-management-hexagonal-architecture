using System.ComponentModel;

namespace TaskManagement.HexagonalArchitecture.Domain.Enums
{
    public enum ProcessReturn
    {
        [Description("Success.")]
        SUCCESS = 12000,
        [Description("E-mail already exists.")]
        ERROR_EMAIL_EXISTS = 12001,

        [Description("Cancelled token.")]
        ERRO_CANCELLED_TOKEN = 99997,
        [Description("Erro de negócio desconhecido.")]
        ERRO_NEGOCIO_DESCONHECIDO = 99998,
        [Description("Erro desconhecido.")]
        ERRO_DESCONHECIDO = 99999
    }
}