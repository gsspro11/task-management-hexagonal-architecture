using Poc.Security.Factors.Options;
using Microsoft.Extensions.Options;

namespace Poc.Security.Factors.Tests
{
    public class TestCommons
    {
        readonly static public string Cpf = "08359004609";
        readonly static public string Valor = "444444";
        readonly static public string Tipo = "TokenSMS";
        readonly static public string Operacao = "AtivacaoTokenOTP";

        readonly static public string BaseUrl = "base/";

        readonly static public IOptions<FactorsOptions> Options = 
            Microsoft.Extensions.Options.Options.Create(new FactorsOptions() {
                ApiBaseUrl = BaseUrl
            });
    }

}
