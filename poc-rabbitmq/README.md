
## IMPORTANTE
Antes de utilizar a solução de RabbitMQ para mensagería, consulte o time de Arquitetura de Soluções.

## Poc.RabbitMQ 
A biblioteca Poc.RabbitMQ é uma ferramenta que facilita o uso do Apache
RabbitMQ em projetos ASP.NET Core, permitindo a produção e o consumo de mensagens de forma fácil e intuitiva.

## Descrição
O Poc.RabbitMQ fornece uma API simples para interagir com o Apache RabbitMQ, oferece uma interface de alto nível para produção e consumo de mensagens, com funcionalidades adicionais como fluxo de retry e dead letter.

Esta biblioteca torna a interação com o RabbitMQ mais simples no contexto do ASP.NET Core, abstraindo a complexidade e oferecendo uma interface amigável para publicação e consumo de mensagens. Ao utilizar em seus projetos, você conseguirá uma integração rápida e eficaz com o RabbitMQ, permitindo focar na lógica do seu negócio.


## Estrutura Básica:
* `PocRabbitMQPubSubBase<T>` : Classe base abstrata que fornece os métodos principais para produzir e consumir mensagens.
* `IPocRabbitMQPubSub<T>`: Interface que define os contratos principais para a publicação e assinatura.
* `PocRabbitMQPubSub<T>`: Implementação concreta do RabbitMQPubSubBase<T>``.
* `ServicesCollectionExtension`: Métodos de extensão para facilitar a injeção de dependências e configuração do publisher e subscriber no ASP.NET Core.
* `PocRabbitMQBrokerSettings`: Representa as configurações do brokiner RabbitMQ. Contém informações como servidores bootstrap, credencias e os tópicos associados.

## Dependências:
* RabbitMQ.Client: Utilizado para comunicação com o RabbitMQ, lib oficial Confluent para .NET
* Microsoft.Extensions.DependencyInjection.Abstractions: Utilizado para extenção do container DI `IServiceCollection`
* Microsoft.Extensions.Logging.Abstractions: Utilizado para registro de logs.
* Microsoft.Extensions.Diagnostics.HealthCheck: Utilizado para registrar um health check, 
para monitorar a saúde do broker RabbitMQ.

## Funcionalidades 
* Publicação de Mensagens: Utilize o método `Publish` para publicar uma única mensagem, ou `BatchPublish` para várias.
* Consumo de Mensagens: Utilize `ConsumeMessage` para consumir uma mensagem, abordando tratamento de erros e re-tentativas. Caso uma mensagem falhe ao ser processada, a biblioteca irá tentar novamente, conforme configurado. Se o limite de tentativas for atingido, a mensagem será enviada para um tópico de mensagens mortas (Dead Letter Topic).


## Instalação 
 Para instalar a lib via NuGet, configure o repositório do Nexus(https://nexus-prd.poc.com.br/repository/nuget-group) execute o seguinte comando no Package
 Manager Console: 
 
 ```bash
 Install-Package Poc.RabbitMQ
```

Ou busque pelo pacote "Poc.RabbitMQ" na interface do NuGet em seu projeto.


### 1. Configuração do `appsetings.json`

Antes de tudo, certifique-se de configurar o arquivo `appsetings.json` correspondente
ao ambiente desejado (por exemplo, `appsetings.Development.json` para desenvolvimento). 

Insira as informações necessárias para conexão e tópicos do RabbitMQ:

```json
{
   "PocRabbitMQBrokerSettings": {
    "Brokers": [
      {
        "Name": "Broker1",
        "UserName": "guest",
        "Password": "guest",
        "VirtualHost": "/",
        "HostName": "localhost",
        "Port": 5672,
        "IsCredentialsProvided": false,
        "IsSsl": false,
        "ClientProvidedName": "app:NomeApp component:event-consumer",
        "RetryCount": 2,
        "Queues": {
          "Broker1Queue": {
            "Queue": "QueueTest",
            "QueueFailed": "QueueTest.failed"
          }
        }
      }
    ]
  }
}
```

> :warning: **Nota Importante**: É possível registrar mais de um broker. O Brokers é um array, basta duplicar a estrutura.



* Objeto de configuração do parametros do componente RabbitMQ
```csharp
public class PocRabbitMQBrokerSettings
{
    public PocRabbitMQBrokerConfig[] Brokers { get; set; }
}

public class PocRabbitMQBrokerConfig
{
    public string Name { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string HostName { get; set; }
    public int Port { get; set; }
    public bool IsSsl { get; set; } = false;

    public Uri Uri { get; set; }
    public int RetryCount { get; set; }
    public int RetryIntervalSeconds { get; set; }


    public Dictionary<string, PocRabbitMQQueueSettings> Queues { get; set; } = null!;
}

public class PocRabbitMQQueueSettings
{
    public string Queue { get; private set; }
    public string QueueFailed { get; set; }
    public ushort MaxConcurrentMessages { get; private set; } = 1;

    public void SetQueue(string queue) => Queue = queue;
    public void SetQueueFailed(string queueFailed) => QueueFailed = queueFailed;
    public void SetMaxConcurrentMessages(ushort maxConcurrentMessages) => MaxConcurrentMessages = maxConcurrentMessages;
}
```


## Exemplos de Uso
Aqui estão alguns exemplos de como usar a lib:

### Exemplo 1: Produzindo mensagens

1. Criar enum de filas 

Criar enum para associação as filas 

 ```csharp

using System.ComponentModel;

public enum RabbitQueue
{
    [Description("QueueTest")]
    Broker1Queue = 0,

}
```

2. Registro das Dependências

Registrar as injeções de dependências no projeto: 

 ```csharp

using Poc.Logging.Internal.Extensions;
using Poc.RabbitMQ;
using Poc.RabbitMQ.Extensions;
using ProducerWorker;
using Shared;
using EnumsNET;
using Poc.Project.Utils.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigurePocLoggingInternal()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddPocServiceProjectDependencies(5000);

        IConfiguration configuration = hostContext.Configuration;

        var rabbitMQBrokerSettings = configuration.GetSection(nameof(PocRabbitMQBrokerSettings)).Get<PocRabbitMQBrokerSettings>();
        foreach (var brokerSettings in rabbitMQBrokerSettings.Brokers)
        {
            var pocRabbitMQ = services.AddPocRabbitMQ<RabbitQueue>(brokerSettings.Name, brokerSettings);

            foreach (var queueSettings in brokerSettings.Queues)
                _ = pocRabbitMQ.AddPocRabbitPubSub<string>(
                    RabbitQueue.Broker1Queue,
                    configAction: config =>
                    {
                        config.SetQueue(RabbitQueue.Broker1Queue.AsString(EnumFormat.Description)!);
                        config.SetMaxConcurrentMessages(1000);
                    });
        }
        // services.AddHostedService<Worker>();
        services.AddHostedService<WorkerSemTipo>();
    })
    .ConfigurePocServiceProjectDependencies()
    .Build();

await host.RunAsync();

```


3. Enviar mensagens 


* Exemplo de um WorkerService 


```csharp
using Poc.Project.Utils.Base;
using Poc.Project.Utils.HealthCheck;
using Poc.RabbitMQ.PubSub;
using Shared;
using System.Text.Json;

namespace ProducerWorker;

public class WorkerSemTipo : PocBackgroundService
{
    private readonly ILogger<WorkerSemTipo> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WorkerSemTipo(
        ILogger<WorkerSemTipo> logger,
        PocWorkerStateService workerStateService,
        IServiceProvider serviceProvider
        ) : base(logger, workerStateService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

    }


    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker is stopping.");
        });

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var resolver = scope.ServiceProvider.GetRequiredService<PocRabbitMQPubSubResolver<RabbitQueue>>();
            var pubSubAccount = (IPocRabbitMQPubSub<string>)resolver(RabbitQueue.Broker1Queue);

            await ExecuteAsync(pubSubAccount,stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");

    }




    protected async Task ExecuteAsync(IPocRabbitMQPubSub<string> pubSubAccount , CancellationToken stoppingToken)
    {

        var serializeOpt = new JsonSerializerOptions();
        serializeOpt.Converters.Add(new CartaoConverterWithTypeDiscriminator());

        for (int j = 1; j <= 1000; j++)
        {

            for (int i = 1; i <= 1000; i++)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                await pubSubAccount.PublishAsync(message: "${ Cvv = 958, Number = '5984 4568 6485 1556' {i.ToString()}}",
                                      serializeOptions: serializeOpt);
            }

           await Task.Delay(4000, stoppingToken);

        }

        pubSubAccount.Dispose();
    }
}


```





### Exemplo 2: Consumindo mensagens

1. Criar enum de filas 

Criar enum para associação as filas 

 ```csharp

using System.ComponentModel;

public enum RabbitQueue
{
    [Description("QueueTest")]
    Broker1Queue = 0,

}
```

2. Registro das Dependências

Registrar as injeções de dependências no projeto:

```csharp

using Poc.Logging.Internal.Extensions;
using Poc.RabbitMQ;
using Poc.RabbitMQ.Extensions;
using ProducerWorker;
using Shared;
using EnumsNET;
using Poc.Project.Utils.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigurePocLoggingInternal()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddPocServiceProjectDependencies(5000);

        IConfiguration configuration = hostContext.Configuration;

        var rabbitMQBrokerSettings = configuration.GetSection(nameof(PocRabbitMQBrokerSettings)).Get<PocRabbitMQBrokerSettings>();
        foreach (var brokerSettings in rabbitMQBrokerSettings.Brokers)
        {
            var pocRabbitMQ = services.AddPocRabbitMQ<RabbitQueue>(brokerSettings.Name, brokerSettings);

            foreach (var queueSettings in brokerSettings.Queues)
                _ = pocRabbitMQ.AddPocRabbitPubSub<string>(
                    RabbitQueue.Broker1Queue,
                    configAction: config =>
                    {
                        config.SetQueue(RabbitQueue.Broker1Queue.AsString(EnumFormat.Description)!);
                        config.SetMaxConcurrentMessages(1000);
                    });
        }
        // services.AddHostedService<Worker>();
        services.AddHostedService<WorkerSemTipo>();
    })
    .ConfigurePocServiceProjectDependencies()
    .Build();

await host.RunAsync();


```

2. Registrar consumidor de mensagens

Registrar um consumidor que observa as mensagens na fila e busca a mesma para processamento invocando uma função.

```csharp

using Poc.RabbitMQ.PubSub;
using Poc.RabbitMQ.Results;
using Poc.Project.Utils.Base;
using Poc.Project.Utils.HealthCheck;
using Shared;



namespace ConsumerWorker;

public class Worker : PocBackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        ILogger<Worker> logger,
        PocWorkerStateService workerStateService,
        IServiceProvider serviceProvider
        ) : base(logger, workerStateService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

    }

    protected override async Task ExecuteServiceAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker is stopping.");
        });


        try
        {
            using var scope = _serviceProvider.CreateScope();
            var resolver = scope.ServiceProvider.GetRequiredService<PocRabbitMQPubSubResolver<RabbitQueue>>();
            var pubSubAccount = (IPocRabbitMQPubSub<string>)resolver(RabbitQueue.Broker1Queue);

            await pubSubAccount.ConsumeMessageManualAckFailedRedirectAsync(onMessageReceived: ProcessMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");


    }

    private async Task<bool> ProcessMessage(PocConsumeResult<string> result)
    {
        try
        {
            _logger.LogInformation(message: "Cartao number: '{result}'", result);

            await Task.Delay(500);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }


        return true;
    }

}
```

## Recomendações
Certifique-se de sempre configurar ambos (produtor e consumidor) adequadamente.


## Licença
Este projeto está sob a licença Copyright © POC SA 2023.

