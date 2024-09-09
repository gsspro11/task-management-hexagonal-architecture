using Poc.Security.Factors.Model;

namespace Poc.Security.Factors
{
    public interface IFactors
    {
        Task<bool> ValidateFactors(Operacao operacao, List<FatorOperacao> fatores, string cpfCliente);
    }
}