using Poc.Security.Factors.Exceptions;
using Poc.Security.Factors.Model.RiskData;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Poc.Security.Factors.RiskDataHandler.Handler
{
    public class PixRiskDataHandler : IRiskDataHandler
    {
        public string GetRiskData(dynamic requestObject)
        {
            //trecho obtido do exemplo flutter
            //md5.convert(DateTime.now().toIso8601String().codeUnits).toString()
            var transactionId = Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(DateTime.Now.ToString("o", CultureInfo.InvariantCulture)))).ToLower();

            var riskData = new PixRiskData() {
                TransactionId = transactionId,
                TransactionDate = DateTime.Now,

                TransactionValue = requestObject.Valor ?? throw new RiskDataMemberNotFoundException("Valor da transacao"),

                RecipientIdNumber = requestObject.Favorecido.Inscricao ?? throw new RiskDataMemberNotFoundException("Inscricao do favorecido"),
                //RecipientIdNumberType = requestObject.Favorecido. ?? throw new RiskDataMemberNotFoundException("Tipo do documento do favorecido"),
                RecipientBranch = requestObject.Favorecido.Agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                RecipientAccountNumber = requestObject.Favorecido.NumeroConta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
                RecipientUserName = requestObject.Favorecido.Nome ?? throw new RiskDataMemberNotFoundException("Nome do favorecido"),
                RecipientInstitutionInfo = requestObject.Favorecido.NomeInstituicao ?? throw new RiskDataMemberNotFoundException("Instituicao do favorecido"),
            };

            return JsonSerializer.Serialize(riskData);
        }
    }
}
