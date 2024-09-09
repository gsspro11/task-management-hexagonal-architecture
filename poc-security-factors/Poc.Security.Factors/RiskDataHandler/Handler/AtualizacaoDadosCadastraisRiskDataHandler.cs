using Poc.Security.Factors.Model.RiskData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poc.Security.Factors.RiskDataHandler.Handler
{
    public class AtualizacaoDadosCadastraisRiskDataHandler : IRiskDataHandler
    {
        public string GetRiskData(dynamic requestObject)
        {
            var riskData =  new AtualizacaoDadosCadastraisRiskData() {
                Endereço = new AtualizacaoDadosCadastraisRiskDataEndereco()
                {
                    Logradouro = "teste"
                }
            };

            return JsonSerializer.Serialize(riskData);
        }
    }
}
