using Poc.Security.Factors.Client;
using Poc.Security.Factors.Exceptions;
using Poc.Security.Factors.Model;
using Poc.Security.Factors.RiskDataHandler;
using Poc.Security.Factors.RiskDataHandler.Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Poc.Security.Factors
{
    public class ValidarTransacaoAttribute : ActionFilterAttribute
    {
        private readonly TipoTransacao _tipoTransacao;
        private readonly string _parameter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transacao conforme Enum TipoTransacao</param>
        /// <param name="parameter">Parametro do Metodo de onde puxará os dados necessários</param>
        public ValidarTransacaoAttribute(string parameter)
        {
            _parameter = parameter;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {

            if (actionContext.HttpContext.Request.ContentType != MediaTypeNames.Application.Json)
            {
                throw new ApplicationException("Body precisa ser Json");
            }

            //objeto que veio no controller
            var requestObject =  actionContext.ActionArguments[_parameter] ?? throw new ApplicationException("Parametro nao encontrado");

            //Montar riskdata:
            try
            {
                var riskData = RiskDataHandlerCollection.GetRiskData(_tipoTransacao, requestObject);

                AvaliarRisco(actionContext, riskData);
            }
            catch (RiskDataMemberNotFoundException ex)
            {
                actionContext.Result = new BadRequestObjectResult(ex.Message);
                return;
            }
            catch (HeaderMemberNotFoundException ex)
            {
                actionContext.Result = new BadRequestObjectResult(ex.Message);
                return;
            }
            catch(Exception ex)
            {
                actionContext.Result = new ObjectResult(new ProblemDetails() {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro",
                    Detail = ex.Message + ' ' + ex.InnerException?.Message
                });
                return;
            }
        }

        private void AvaliarRisco(ActionExecutingContext actionContext, string riskData)
        {
            //obter os dados para a requisicao da api
            var heartbeat = actionContext.HttpContext.Request.Headers["heartbeat"].FirstOrDefault() ?? throw new HeaderMemberNotFoundException("heartbeat");
            var enderecoIp = actionContext.HttpContext.Request.Headers["enderecoIP"].FirstOrDefault() ?? throw new HeaderMemberNotFoundException("enderecoIP");
            var cpfCliente = actionContext.HttpContext.Request.Headers["cpfCliente"].FirstOrDefault() ?? throw new HeaderMemberNotFoundException("cpfCliente");
            var origem = "Origem"; //Todo

            //bater na api para avaliar o risco
            var client = actionContext.HttpContext.RequestServices.GetService<ISegurancaApiClient>() ?? throw new Exception("ISegurancaApiClient nao registrado no DI. Esqueceu de chamar AddPocFactors?");
            client.FluxoAutenticacao(heartbeat, enderecoIp, riskData, origem, cpfCliente);
        }
    }
}
