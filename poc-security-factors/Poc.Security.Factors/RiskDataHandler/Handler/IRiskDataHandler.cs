using Poc.Security.Factors.Model.RiskData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Security.Factors.RiskDataHandler.Handler
{
    public interface IRiskDataHandler
    {
        public string GetRiskData(dynamic requestObject);
        public string GetFluxAuthentication(dynamic requestObject);
        public string GetValidateFactorAuthentication(dynamic requestObject);
        public string GetValidateFactorOperation(dynamic requestObject);
        public string GetRollbackValidateFactorOperation(dynamic requestObject);
        public string GetValidateFactor(dynamic requestObject);
        public string GetSendTokenMail(dynamic requestObject);
        public string GetSendTokenMessage(dynamic requestObject);
    }
}
