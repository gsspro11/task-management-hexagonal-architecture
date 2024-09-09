using Poc.Security.Factors.Model;
using Poc.Security.Factors.Model.RiskData;
using Poc.Security.Factors.RiskDataHandler.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Poc.Security.Factors.RiskDataHandler
{
    internal static class RiskDataHandlerCollection
    {
        private static readonly Dictionary<TipoTransacao, IRiskDataHandler> _riskDataHandlers = new();

        internal static void AddHandler<T>(TipoTransacao tipoTransacao) where T: IRiskDataHandler, new()
        {
            if (!_riskDataHandlers.ContainsKey(tipoTransacao))
                _riskDataHandlers.Add(tipoTransacao, new T());
        }

        public static string GetRiskData(TipoTransacao tipoTransacao, dynamic requestObject)
        {
            if(!_riskDataHandlers.ContainsKey(tipoTransacao))
                throw new ArgumentOutOfRangeException("Este tipo de transacao nao esta registrado. Faltou chamar AddHandler?");
            
            return _riskDataHandlers[tipoTransacao].GetRiskData(requestObject);
        }
    }
}
