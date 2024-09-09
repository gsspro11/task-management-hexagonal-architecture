using Poc.Security.Factors;
using Poc.Security.Factors.Model;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteController : ControllerBase
    {

        private readonly ILogger<TesteController> _logger;

        public TesteController(
            ILogger<TesteController> logger
        )
        {
            _logger = logger;
        }

        [HttpPost("pix")]
        [ValidarTransacao("pix")]
        public async Task<ActionResult> TestePix(RequestPix pix)
        {

            return Ok();
        }

        [HttpPost("atualizacao-dados-cadastrais")]
        [ValidarTransacao("dados")]
        public async Task<ActionResult> TesteAtualizacaoDadosCadastrais(object dados)
        {
            return Ok();
        }
    }
}