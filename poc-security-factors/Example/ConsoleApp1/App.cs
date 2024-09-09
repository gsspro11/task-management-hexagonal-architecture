using Poc.Security.Factors;
using Poc.Security.Factors.Model;

namespace ConsoleApp1
{
    public class App
    {
        private readonly IFactors _factors;
        public App(IFactors factors)
        {
            _factors = factors;
        }

        public async Task AppRun()
        {
            Console.WriteLine("Validacao de Fatores");

            Console.WriteLine("Digite o CPF");

            var cpf = Console.ReadLine();

            var fatores = new List<FatorOperacao>();
            var i = 1;

            while (true)
            {
                Console.WriteLine("Digite o fator " + i + " ou enter para validar");

                var fatorLido = Console.ReadLine();

                if(fatorLido == "")
                {
                    break;
                }

                Console.WriteLine("Digite o tipo do fator" + i);
                var tipoLido = Console.ReadLine();

                fatores.Add(new FatorOperacao() { 
                    Jwt = fatorLido,
                    Tipo = tipoLido
                });

                i++;
            }

            var valido = await _factors.ValidateFactors(Operacao.AtivacaoTokenOTP, fatores, cpf);

            if (valido)
                Console.WriteLine("Fatores Validos!");
            else
                Console.WriteLine("Fatores Invalidos");


        }

    }
}
