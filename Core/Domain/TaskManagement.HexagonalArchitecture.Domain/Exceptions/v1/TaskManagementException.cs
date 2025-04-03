namespace TaskManagement.HexagonalArchitecture.Application.Common.Exceptions.v1
{
    public sealed class TaskManagementDomainException : TaskManagementCustomException
    {
        public TaskManagementDomainException(int statusCode = 400)
            : base(statusCode)
        {
            base.Dados = new
            {
                Mensagem = "Erro de negócio."
            };
        }

        public TaskManagementDomainException(object dados, int statusCode = 400)
            : base(dados, statusCode)
        {
        }

        public TaskManagementDomainException(string mensagem, int statusCode = 400)
            : base(mensagem, statusCode)
        {
        }

        public TaskManagementDomainException(object dados, Exception innerException, int statusCode = 500)
            : base(dados, innerException, statusCode)
        {
        }

        public TaskManagementDomainException(string mensagem, Exception innerException, int statusCode = 500)
            : base(mensagem, innerException, statusCode)
        {
        }
    }

    public class TaskManagementCustomException : Exception
    {
        public int StatusCode { get; protected set; }

        public object Dados { get; set; }

        public TaskManagementCustomException(int statusCode = 500)
        {
            StatusCode = statusCode;
            Dados = new
            {
                Mensagem = Message
            };
        }

        public TaskManagementCustomException(object dados, int statusCode = 500)
        {
            StatusCode = statusCode;
            Dados = dados;
        }

        public TaskManagementCustomException(object dados, Exception innerException, int statusCode = 500)
            : base("", innerException)
        {
            StatusCode = statusCode;
            Dados = dados;
        }

        public TaskManagementCustomException(string mensagem, int statusCode = 500)
            : base(mensagem)
        {
            StatusCode = statusCode;
            Dados = new
            {
                Mensagem = mensagem
            };
        }

        public TaskManagementCustomException(string mensagem, Exception innerException, int statusCode = 500)
            : base(mensagem, innerException)
        {
            StatusCode = statusCode;
            Dados = new
            {
                Mensagem = mensagem
            };
        }
    }
}
