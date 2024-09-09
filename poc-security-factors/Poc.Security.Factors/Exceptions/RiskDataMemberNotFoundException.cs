using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Security.Factors.Exceptions
{
    public class RiskDataMemberNotFoundException : Exception
    {
        public RiskDataMemberNotFoundException(string? member) : base("Erro ao montar RiskData. Membro nao encontrado: " + member)
        {
        }
    }
}
