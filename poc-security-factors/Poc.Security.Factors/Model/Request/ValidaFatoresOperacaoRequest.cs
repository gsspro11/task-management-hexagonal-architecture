using Poc.Security.Factors.Model;

namespace Poc.Security.Factors.Model.Request
{
    public class ValidaFatoresOperacaoRequest
    {
        public IEnumerable<FatorOperacao> Fatores { get; set; }
        public Operacao Valor { get; set; }
    }

}
