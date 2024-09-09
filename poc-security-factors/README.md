# Poc.Auth
[![Net](https://img.shields.io/badge/6.0.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0)
[![C#](https://img.shields.io/static/v1?label=C%20Sharp&message=10.0.0&color=blue&style=for-the-badge&logo=c-sharp)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/)
[![Nuget](https://img.shields.io/static/v1?label=nuget&message=Dependencies&color=blue&style=for-the-badge&logo=NUGET)](https://nexus-prd.poc.com.br/#browse/browse:nuget-releases)
[![C#](https://img.shields.io/static/v1?label=AZURE&message=DEVOPS&color=blue&style=for-the-badge&logo=azure)](https://azure.microsoft.com/pt-br/products/devops)

## Tags
Segurança | Factor | Security

## Sobre o projeto
 Este pacote é responsável por gerenciar os fatores de segurança das transações do Poc.

## Funcionalidades
- Validação dos riscos de segurança das transações do Poc.

## Pré-requisitos
 [![Net](https://img.shields.io/badge/6.0.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0)

## Definições de uso
O uso do componente **Poc.Security.Factors** é uma solução de encapsular chamadas de API de segurança de risco das transações do Poc.

## Como Utilizar o Pacote
1. Após criar o projeto, instale o pkg **Poc.Security.Factors** via Nexus apontando para o source Poc.
2. Abra o arquivo *program.cs* do seu projeto:
2.1 Inserir o import da lib do Poc.Security.Factors para uso das extensions, conforme exemplo abaixo:
```cs
    using Poc.Security.Factors;;  //Importação do componente Poc.Security.Factors
    
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
```
2.2 Inserir a chamada "builder.Services.AddPocFactors();"
Exemplo:
```cs
    {
        ...
        ...
        ...
        builder.Services.AddPocFactors(o => {});
        ...
        ...
        ...
        var app = builder.Build();
    }
```

3. Para realizar as chamadas, inserir a anotação na controller e enviar o tipo de chamada a ser realizada. 
**Exemplo:**  Importante adequar ao tipo de chamada da sua controller.
```cs
    [ValidarTransacao("tipo_de_transacao")] //Informar qual a transação
    public async Task<ActionResult> Teste(Request request)
    {
        return Ok(); // Em caso positivo retornar a chamada 
    }
```

**Exemplo Aplicado a uma transação PIX e Atualização Cadastral**

```cs
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
            
            public TesteController(ILogger<TesteController> logger)
            {
                _logger = logger;
            }
    
            //Transação PIX
            [HttpPost("pix")]
            [ValidarTransacao("pix")]
            public async Task<ActionResult> TestePix(RequestPix pix)
            {
                return Ok();
            }
            
            //Transação Atualização Cadastral
            [HttpPost("atualizacao-dados-cadastrais")]
            [ValidarTransacao("dados")]
            public async Task<ActionResult> TesteAtualizacaoDadosCadastrais(object dados)
            {
                return Ok();
            }
        }
    }
```

**Atenção:** Passar a classe da requisição de acordo com os parâmetros da API, seguindo o swagger da API de segurança que você pode acessar pelo link [Swagger ACAS](  http://nlb-segurancatransacao-hml-d119dc28a8f3347f.elb.us-east-2.amazonaws.com:8080/swagger/#/Seguranca/FatorController_postAuthenticationFlow.)

3.1 Em caso de retorno com **SUCESSO**, será refletido em sua controller este continuará com sua chamada normalmente.

3.2 Em caso de **ERRO**, retornará a exception com as informações que motivaram a não autorização de acordo com a avaliação de risco. 


Última atualização 18/04/2024


