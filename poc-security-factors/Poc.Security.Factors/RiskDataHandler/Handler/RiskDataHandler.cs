using Poc.Security.Factors.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poc.Security.Factors.RiskDataHandler.Handler
{
    internal class RiskDataHandler : IRiskDataHandler
    {
        public string GetFluxAuthentication(dynamic requestObject)
        {
            var FluxAuthentication = new Dictionary<string, object>
            {
                ["TransactionValue"] = requestObject.Valor ?? throw new RiskDataMemberNotFoundException("Valor da transacao"),
                ["RecipientIdNumber"] = requestObject.Favorecido.Inscricao ?? throw new RiskDataMemberNotFoundException("Inscricao do favorecido"),
                ["RecipientBranch"] = requestObject.Favorecido.Agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["RecipientAccountNumber"] = requestObject.Favorecido.NumeroConta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
                ["RecipientUserName"] = requestObject.Favorecido.Nome ?? throw new RiskDataMemberNotFoundException("Nome do favorecido"),
                ["RecipientInstitutionInfo"] = requestObject.Favorecido.NomeInstituicao ?? throw new RiskDataMemberNotFoundException("Instituicao do favorecido"),
            };

            return JsonSerializer.Serialize(FluxAuthentication);
        }

        public string GetRiskData(dynamic requestObject)
        {
            var riskFactor = new Dictionary<string, object>
            {
                ["TransactionValue"] = requestObject.Valor ?? throw new RiskDataMemberNotFoundException("Valor da transacao"),
                ["RecipientIdNumber"] = requestObject.Favorecido.Inscricao ?? throw new RiskDataMemberNotFoundException("Inscricao do favorecido"),
                ["RecipientBranch"] = requestObject.Favorecido.Agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["RecipientAccountNumber"] = requestObject.Favorecido.NumeroConta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
                ["RecipientUserName"] = requestObject.Favorecido.Nome ?? throw new RiskDataMemberNotFoundException("Nome do favorecido"),
                ["RecipientInstitutionInfo"] = requestObject.Favorecido.NomeInstituicao ?? throw new RiskDataMemberNotFoundException("Instituicao do favorecido"),
            };

            return JsonSerializer.Serialize(riskFactor);
        }

        public string GetRiskFluxAuthentication(dynamic requestObject)
        {
            var FluxAuthentication = new Dictionary<string, object>
            {
                ["TransactionFactor"] = requestObject.fator ?? throw new RiskDataMemberNotFoundException("fator da transacao"),
                ["TransactionValue"] = requestObject.valor ?? throw new RiskDataMemberNotFoundException("Valor da transação"),
                ["TransactionAgence"] = requestObject.agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["TransactionCont"] = requestObject.conta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
            };

            return JsonSerializer.Serialize(FluxAuthentication);
        }

        public string GetRollbackValidateFactorOperation(dynamic requestObject)
        {
            var RollbackValidateFactorOperation = new Dictionary<string, object>
            {
                ["TransactionFactor"] = requestObject.fator ?? throw new RiskDataMemberNotFoundException("fator da transacao"),
                ["TransactionValue"] = requestObject.valor ?? throw new RiskDataMemberNotFoundException("Valor da transação"),
                ["TransactionAgence"] = requestObject.agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["TransactionCont"] = requestObject.conta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
            };

            return JsonSerializer.Serialize(RollbackValidateFactorOperation);
        }

        public string GetSendTokenMail(dynamic requestObject)
        {
            var SendTokenMail = new Dictionary<string, object>
            {
                ["email"] = requestObject.email ?? throw new RiskDataMemberNotFoundException("E-mail"),
                ["canalCampanha"] = requestObject.canalCampanha ?? throw new RiskDataMemberNotFoundException("Canal campanha"),
                ["nomeCompleto"] = requestObject.nomeCompleto ?? throw new RiskDataMemberNotFoundException("Nome completo"),
                ["reenvio"] = requestObject.reenvio ?? throw new RiskDataMemberNotFoundException("Status Reenvio"),
            };

            return JsonSerializer.Serialize(SendTokenMail);
        }

        public string GetSendTokenMessage(dynamic requestObject)
        {
            var SendTokenMessage = new Dictionary<string, object>
            {
                ["audioToken"] = requestObject.reenvio ?? throw new RiskDataMemberNotFoundException("Valor da transacao"),
                ["reenvio"] = requestObject.reenvio ?? throw new RiskDataMemberNotFoundException("Inscricao do favorecido")
            };

            return JsonSerializer.Serialize(SendTokenMessage);
        }

        public string GetValidateFactor(dynamic requestObject)
        {
            var ValidateFactor = new Dictionary<string, object>
            {
                ["TransactionValue"] = requestObject.Valor ?? throw new RiskDataMemberNotFoundException("Valor da transacao"),
                ["RecipientIdNumber"] = requestObject.Favorecido.Inscricao ?? throw new RiskDataMemberNotFoundException("Inscricao do favorecido"),
                ["RecipientBranch"] = requestObject.Favorecido.Agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["RecipientAccountNumber"] = requestObject.Favorecido.NumeroConta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
                ["RecipientUserName"] = requestObject.Favorecido.Nome ?? throw new RiskDataMemberNotFoundException("Nome do favorecido"),
                ["RecipientInstitutionInfo"] = requestObject.Favorecido.NomeInstituicao ?? throw new RiskDataMemberNotFoundException("Instituicao do favorecido"),
            };

            return JsonSerializer.Serialize(ValidateFactor);
        }

        public string GetValidateFactorAuthentication(dynamic requestObject)
        {
            var ValidateFactorAuthentication = new Dictionary<string, object>
            {
                ["TransactionFactor"] = requestObject.fator ?? throw new RiskDataMemberNotFoundException("fator da transacao"),
                ["TransactionValue"] = requestObject.valor ?? throw new RiskDataMemberNotFoundException("Valor da transação"),
                ["TransactionAgence"] = requestObject.agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["TransactionCont"] = requestObject.conta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
            };

            return JsonSerializer.Serialize(ValidateFactorAuthentication);
        }

        public string GetValidateFactorOperation(dynamic requestObject)
        {
            var ValidateFactorOperation = new Dictionary<string, object>
            {
                ["TransactionFactor"] = requestObject.fator ?? throw new RiskDataMemberNotFoundException("fator da transacao"),
                ["TransactionValue"] = requestObject.valor ?? throw new RiskDataMemberNotFoundException("Valor da transação"),
                ["TransactionAgence"] = requestObject.agencia ?? throw new RiskDataMemberNotFoundException("Agencia do favorecido"),
                ["TransactionCont"] = requestObject.conta ?? throw new RiskDataMemberNotFoundException("Conta do favorecido"),
            };

            return JsonSerializer.Serialize(ValidateFactorOperation);
        }
    }
}
