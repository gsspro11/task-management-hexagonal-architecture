using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Security.Factors.Exceptions
{
    public class HeaderMemberNotFoundException : Exception
    {
        public HeaderMemberNotFoundException(string? member) : base("Erro ao montar Requisicao. Membro nao encontrado encontrado no header: " + member)
        {
        }
    }
}
