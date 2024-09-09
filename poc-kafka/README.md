# Poc.Kafka
## _Componente para Mensagería_

[![Net](https://img.shields.io/badge/6.0.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0)
[![C#](https://img.shields.io/static/v1?label=C%20Sharp&message=10.0.0&color=blue&style=for-the-badge&logo=c-sharp)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/)
[![Nuget](https://img.shields.io/static/v1?label=nuget&message=Dependencies&color=blue&style=for-the-badge&logo=NUGET)](https://nexus-prd.poc.com.br/#browse/browse:nuget-releases)
[![AWS](https://img.shields.io/badge/Amazon_AWS-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white)](https://docs.aws.amazon.com/pt_br/systems-manager/latest/userguide/systems-manager-parameter-store.html)
[![Kafka](https://img.shields.io/badge/kafka-mks-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white)](https://aws.amazon.com/pt/msk/)

## Tags
MKS | Kafka | Apache | Mensageria | AWS

## Sobre o projeto
A biblioteca Poc.Kafka é uma ferramenta que facilita o uso do Apache
Kafka em projetos ASP.NET Core, permitindo a produção e o consumo de mensagens de forma fácil e intuitiva.

O Poc.Kafka fornece uma API simples para interagir com o Apache Kafka, oferece uma interface de alto nível para produção e consumo de mensagens, com funcionalidades adicionais como fluxo de retry e dead letter.

Esta biblioteca torna a interação com o Kafka mais simples no contexto do ASP.NET Core, abstraindo a complexidade e oferecendo uma interface amigável para publicação e consumo de mensagens. Ao utilizar em seus projetos, você conseguirá uma integração rápida e eficaz com o Kafka, permitindo focar na lógica do seu negócio.

## Funcionalidades 
* Publicação de Mensagens: Utilize o método `SendAsync` para publicar uma única mensagem, ou `SendBatchAsync` para várias.
* Consumo de Mensagens: Utilize `ConsumeAsync` para consumir uma mensagem, abordando tratamento de erros e re-tentativas. Caso uma mensagem falhe ao ser processada, a biblioteca irá tentar novamente, conforme configurado. Se o limite de tentativas for atingido, a mensagem será enviada para um tópico de mensagens mortas (Dead Letter Topic).

## Estrutura Básica:
* `PocKafkaPubSubBase<TKey, TValue>` : Classe base abstrata que fornece os métodos principais para produzir e consumir mensagens.
* `IPocKafkaPubSub<TKey, TValue>`: Interface que define os contratos principais para a publicação e assinatura.
* `PocKafkaPubSub<TKey, TValue>`: Implementação concreta do KafkaPubSubBase<TKey, TValue>``.
* `ServicesCollectionExtension`: Métodos de extensão para facilitar a injeção de dependências e configuração do publisher e subscriber no ASP.NET Core.
* `PocKafkaBrokerSettings`: Representa as configurações do broker Kfaka. Contém informações como servidores bootstrap, credencias e os tópicos associados.

## Dependências:
* Confluent.Kafka: Utilizado para comunicação com o Kafka, lib oficial Confluent para .NET
* Microsoft.Extensions.DependencyInjection.Abstractions: Utilizado para extenção do container DI `IServiceCollection`
* Microsoft.Extensions.Logging.Abstractions: Utilizado para registro de logs.
* Microsoft.Extensions.Diagnostics.HealthCheck: Utilizado para registrar um health check, 
para monitorar a saúde do broker Kafka.

## Instalação 
 Para instalar a lib via NuGet, configure o repositório do Nexus(https://nexus-prd.poc.com.br/repository/nuget-group) execute o seguinte comando no Package
 Manager Console: 
 
 ```bash
 Install-Package Poc.Kafka
```

Ou busque pelo pacote "Poc.Kafka" na interface do NuGet em seu projeto.

## Exemplos de Uso
Aqui estão alguns exemplos de como usar a lib:

### Exemplo 1: Produzindo mensagens

1. Configuração do `appsetings.json`

Antes de tudo, certifique-se de configurar o arquivo `appsetings.json` correspondente
ao ambiente desejado (por exemplo, `appsetings.Development.json` para desenvolvimento). 

Insira as informações necessárias para conexão e tópicos do Kafka:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "PocKafkaBrokerSettings": {
    "BrokerCoreBanking": {
      "BootstrapServer": "b-1....:9092,b-2....:9092,b-3....:9092",
      "Username": "username",
      "Password": "password",
      "Topics": {
        "TOPIC-EXAMPLE-PRODUCER": { 
          "MessageSendMaxRetries": 3,
          "MaxInFlight": 1
          // "NoConfig": true
        }
      }
    }
  }
}

```
> :warning: **Nota Importante**: `"NoConfig": true` Se não for necessário sobrescrever nenhuma 
configuração, mantenha apenas a propriedade "NoConfig": true.".


> :warning: **Nota Importante**: É possível registrar mais de um broker. 
Apenas siga a estrutura do "Broker_1" para adicionar mais entradas.

#### Configurações do Producer

* BootstrapServer
    * **O que é?** O servidor ao qual o produtor deve se conectar. 
    * **Por que é importante?** Indica onde as mensagens devem ser enviadas. Uma configuração incorreta pode resultar em falhas de publicação.
    
* Username e Password
    * **O que é?** Credenciais para autenticação.
    * **Por que é importante?** Em ambientes protegidos por senha, essas credenciais garantem que o produtor possa se conectar e enviar mensagens ao broker.

* Topics
    * **O que é?** Os tópicos para os quais o produtor enviará mensagens.
    * **Por que é importante?** Os tópicos organizam as mensagens e garantem que elas sejam enviadas aos consumidores adequados.

* SetIdempotenceEnabled
    * **O que é?** Uma configuração que garante que as mensagens sejam entregues exatamente uma vez.
    * **Por que é importante?** Previne duplicatas na entrega de mensagens, garantindo a consistência dos dados. No entanto, pode reduzir ligeiramente o desempenho.

* MessageSendMaxRetries
    * **O que é?** O número máximo de tentativas que o produtor fará para enviar uma mensagem.
    * **Por que é importante?** Em casos de falhas temporárias, essa configuração permite que o produtor tente enviar a mensagem novamente. Limitar as tentativas evita repetições infinitas.

* MaxInFlight
    * **O que é?** Número máximo de mensagens que podem estar em trânsito (não confirmadas) simultaneamente.
    * **Por que é importante?** Garante que, em modo idempotente, não haja mais mensagens em trânsito do que o sistema possa confirmar, reduzindo o risco de perda de mensagens.

2. Registro das Dependências

O próximo passo é registrar as injeções de dependências no projeto: 
 ```csharp

IHost host = Host.CreateDefaultBuilder(args)
    .UseEnvironment("Staging")
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        var kafkaBrokerSettings = configuration.GetSection(nameof(PocKafkaBrokerSettings)).Get<Dictionary<string, PocKafkaBrokerSettings>>();
        foreach (var brokerSettings in kafkaBrokerSettings)
        {
            var pockafka = services
             .AddPocKafka(brokerSettings.Key, brokerSettings.Value.BootstrapServer!, brokerSettings.Value.Username!, brokerSettings.Value.Password!);

            foreach (var topicSettings in brokerSettings.Value.Topics)
                pockafka.AddPocKafkaPubSub<string, CartaoMessage>(
                   producerConfigAction: config =>
                   {
                       config.SetIdempotenceEnabled();
                       config.SetTopic(topicSettings.Key);
                   });
        }
 
        services.AddHostedService<Worker>();
    })
    .ConfigurePocLoggingInternal()
    .Build();

await host.RunAsync();
```

> :warning: **Nota Importante**: A idempotência é uma propriedade fundamental
quando lidamos com mensagens em sistema distribuídos, pois garante que mesmo em
caso de falhas ou reenvios, a operação terá o mesmo resultado. Quando ativar `config.SetIdempotenceEnabled();`, recomenda-se combinar as configurações `"MessageSendMaxRetries"`: 3 e `"MaxInFlight"`: 1. O Valor de `MaxInFlight` pode variar entre 1 e 5, enquanto `MessageSendMaxRetries` não pode ser maior que 3. Tenha em mente que ao usar a idempotência, as operações podem se tornar mais lentas, visto que há verificações adicionais para garantir que cada mensagem seja única.


4. Exemplo de Uso

Para ilustrar a aplicação prática das configurações e serviços, veja um exemplo
usando um projeto `WorkerService` que publica mensagens para um tópico Kafka:

```csharp
public class Worker : BackgroundService 
{
    private readonly IPocKafkaPubSub<string, CartaoMessage> _pubSubCartao;
    public Worker(IPocKafkaPubSub<string, CartaoMessage> pubSubCartao)
    {
        _pubSubCartao = pubSubCartao;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        int messagesToSend = 1000;
        for (int i = 1; i <= messagesToSend; i++)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            string key = i.ToString();
            _ = await _pubSubCartao.SendAsync(message: new CartaoMessage() { Cvv = "958", Number = "5984 4568 6485 1556" }, key: key, cancellationToken: stoppingToken);

            _pubSubCartao.Dispose();
        }
    }
}
```

### Exemplo 2: Consumindo mensagens

1. Configuração do `appsetings.json`

Antes de tudo, certifique-se de configurar o arquivo `appsetings.json` correspondente 
ao ambiente desejado (por exemplo, `appsetings.Development.json` para desenvolvimento). 

Insira as informações necessárias para conexão e tópicos do Kafka:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "PocKafkaBrokerSettings": {
    "BrokerCoreBanking": {
      "Username": "username",
      "Password": "password",
      "BootstrapServer": "b-1....:9092,b-2....:9092,b-3....:9092",
      "Topics": {
        "TOPIC-EXAMPLE-CONSUMER": {
          "TopicRetry": "TOPIC-EXAMPLE-CONSUMER-RETRY",
          "TopicDeadLetter": "TOPIC-EXAMPLE-CONSUMER-DLQ",
          "MessageSendMaxRetries": 3,
          "MaxInFlight": 5,
          "GroupId": "consumer-example-group",
          "SessionTimeoutMs": 30000,
          "RetryLimit": 3,
          "DelayPartitionEofMs": 1000,
          "MaxConcurrentMessages": 5
        },
        "TOPIC-EXAMPLE-PRODUCER": {  // Opcional
          "NoConfig": true
        }
      }
    }
  }
}
```
> :warning: **Nota Importante**: É possível registrar mais de um broker. 
Apenas siga a estrutura do "Broker_1" para adicionar mais entradas.


#### Configuração do Consumer

* BootstrapServer
    * **O que é?** O servidor ao qual o produtor deve se conectar.
    * **Por que é importante?** Indica onde as mensagens devem ser enviadas. Uma configuração incorreta pode resultar em falhas de publicação.

* Username e Password
    * **O que é?** Credenciais para autenticação.
    * **Por que é importante?** Em ambientes protegidos por senha, essas credenciais garantem que o produtor possa se conectar e enviar mensagens ao broker.

* GroupId
    * **O que é?** Indentificador exclusivo para o grupo de consumidores.
    * **Por que é importante?** Permite que múltiplas instâncias do consumidor processem mensagens em paralelo, 
    aumentando a eficiência e assegurando que cada mensagem seja processada uma única vez. 

* Topic, TopicRetry e TopicDeadLetter
    * **O que é?** Configurações relacionadas a tópicos onde mensagens são lidas e, em caso de falhas, redirecionadas.
    * **Por que é importante?** Garante que as mensagens sejam 
    consumidas do tópico correto e, se não fore processadas com 
    sucesso após várias tentativas, sejam movidas para tópicos 
    específicos para análise posterior. 

* RetryLimit
    * **O que é?** Define o número máximo de tentativas para processar uma mensagem.
    * **Por que é importante?** Evita o processamento infinito de mensagens problemáticas. 
    Se uma mensagem não for processada com sucesso após as tentativas especificadas, 
    ela será descartada ou movida para um tópico de dead letter, dependendo da configuração.

* MaxConcurrentMessages
    * **O que é?** Número máximo de mensagens processadas em paralelo.
    * **Por que é importante?** Controla a carga de trabalho, garantindo que o sistema não fique sobrecarregado ao processar muitas mensagens simultaneamente.

* DelayIPartitionEofMs
    * **O que é?** O tempo de espera quando o consumer alcança o final de uma partição.
    * **Por que é importante?** Evita consultas excessivas ao broker, economizando recursos. O valor mínimo é 1000ms (1 segundo).

* FetchWaitMaxMs
    * **O que é?** Tempo máximo que o broker pode esperar para preencher a resposta Fetch com fetch.min.bytes de mensagens.
    * **Por que é importante?** Permite que o broker acumule dados suficientes para reduzir o número de respostas vazias ou pequenas, otimizando a eficiência da rede e do consumo. O valor padrão é 500ms.

* FetchMinBytes
    * **O que é?**  Número mínimo de bytes que o broker responde em uma solicitação de Fetch. Se o tempo fetch.wait.max.ms expirar, os dados acumulados serão enviados ao cliente independentemente dessa configuração.
    * **Por que é importante?** Configurar este valor para mais de 1 pode aumentar a eficiência do broker e do cliente ao reduzir o número de chamadas de rede necessárias para buscar dados, melhorando o throughput geral. O valor padrão é 1 byte.

* FetchMaxBytes
    * **O que é?**  Quantidade máxima de dados que o broker retorna para uma solicitação de Fetch. As mensagens são buscadas em lotes pelo consumidor, e se o primeiro lote de mensagens na primeira partição não vazia da solicitação de Fetch for maior do que este valor, o lote de mensagens ainda será retornado para garantir o progresso do consumidor.
    * **Por que é importante?** Limita o tamanho dos dados retornados pelo broker em cada solicitação de Fetch, prevenindo o uso excessivo de memória e garantindo que o consumidor possa processar os dados eficientemente. O valor padrão é 52.428.800 bytes (50MB).

* MaxPartitionFetchBytes
    * **O que é?**  Número inicial máximo de bytes por tópico+partição para solicitar ao buscar mensagens do broker. Se o cliente encontrar uma mensagem maior que esse valor, ele tentará aumentá-lo gradualmente até que a mensagem inteira possa ser buscada.
    * **Por que é importante?** Este valor define o limite superior para o tamanho dos lotes de mensagens que podem ser buscados por partição, impactando o uso de memória do consumidor e a eficiência do processamento. Um valor muito baixo pode aumentar o número de solicitações ao broker, enquanto um valor muito alto pode levar a um uso excessivo de memória. O valor padrão é 1.048.576 bytes (1MB).


2. Registro das Dependências

O próximo passo é registrar as injeções de dependências no projeto:

```csharp
IHost host = Host.CreateDefaultBuilder(args)
           .UseEnvironment("Staging")
           .ConfigureServices((hostContext, services) =>
           {
               IConfiguration configuration = hostContext.Configuration;

               var kafkaBrokerSettings = configuration
               .GetSection(nameof(PocKafkaBrokerSettings))
               .Get<Dictionary<string, PocKafkaBrokerSettings>>();

               foreach (var brokerSetting in kafkaBrokerSettings)
               {
                   var kafka = services.AddPocKafka<KafkaTopic>(
                       brokerSetting.Key,
                       brokerSetting.Value.BootstrapServer!,
                       brokerSetting.Value.Username!,
                       brokerSetting.Value.Password!);

                   foreach (var topicSettings in brokerSetting.Value.Topics)
                   {
                       if (topicSettings.Key.Equals(KafkaTopic.ExampleConsumer.AsString(EnumFormat.Description),StringComparison.OrdinalIgnoreCase))
                           kafka.AddPocKafkaPubSub<string, MessageExampleConsumer>(
                               KafkaTopic.ExampleConsumer,
                               consumerConfigAction: consumerConfig =>
                               {
                                   consumerConfig.SetTopic(topicSettings.Key);
                                   consumerConfig.SetGroupId(topicSettings.Value.GroupId!);
                                   consumerConfig.SetMaxConcurrentMessages(topicSettings.Value.MaxConcurrentMessages!);
                                   consumerConfig.SetRetryLimit(topicSettings.Value.RetryLimit!);
                                   consumerConfig.SetTopicRetry(topicSettings.Value.TopicRetry!);
                                   consumerConfig.SetSessionTimeoutMs(topicSettings.Value.SessionTimeoutMs!);
                                   consumerConfig.SetEnableRetryTopicSubscription();
                                   consumerConfig.SetAutoOffsetReset(AutoOffsetReset.Earliest);
                                   // Add any other consumer settings as needed
                               });


                       if (topicSettings.Key.Equals(KafkaTopic.ExampleProducer.AsString(EnumFormat.Description), StringComparison.OrdinalIgnoreCase))
                           kafka.AddPocKafkaPubSub<string, MessageExampleProducer>(
                               KafkaTopic.ExampleProducer,
                               producerConfigAction: producerConfig =>
                               {
                                   producerConfig.SetTopic(topicSettings.Key);
                                   producerConfig.SetIdempotenceEnabled(); // Optional
                                   // Add any other producer settings as needed
                               });
                   }
               }

               services.AddHostedService<Worker>();
           })
           .ConfigurePocLoggingInternal()
           .Build();

await host.RunAsync();

```

> :warning: **Nota Importante**: Sobre o `config.SetEnableRetryTopicSubscription()`
Quando você habilita a retentativa de inscrição no tópico usando SetRetryTopicSubscriptionEnabled(), é essencial definir TopicRetry. 
Com essa configuração, mensagens que falham repetidamente são movidas para um tópico de retentativa, onde podem ser processadas novamente 
ou analisadas para identificar problemas pelo próprio woker. 

4. Exemplo de Uso

Para ilustrar a aplicação prática das configurações e serviços, veja um exemplo
usando um projeto `Worker` que publica mensagens para um tópico Kafka:

```csharp

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPocKafkaPubSub<string, MessageExampleConsumer> _consumer;
    private readonly IPocKafkaPubSub<string, MessageExampleProducer> _producer;

    public Worker(
        ILogger<Worker> logger,
        PocKafkaPubSubResolver<KafkaTopic> kafkaPubsSubsResolver)
    {
        _logger = logger;
        _consumer = (IPocKafkaPubSub<string, MessageExampleConsumer>)kafkaPubsSubsResolver(KafkaTopic.ExampleConsumer);
        _producer = (IPocKafkaPubSub<string, MessageExampleProducer>)kafkaPubsSubsResolver(KafkaTopic.ExampleProducer);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        stoppingToken.Register(() =>
        {
            _logger.LogInformation("Worker is stopping.");
        });

        try
        {
            await _consumer.ConsumeAsync(onMessageReceived: ProcessMessageAsync, cancellationToken: stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while consuming messages.");
        }

        _logger.LogInformation("Worker has stoped");

    }
    private async Task ProcessMessageAsync(PocConsumeResult<string, MessageExampleConsumer> consumeResult)
    {
        try
        {
            _logger.LogInformation(
                message: "Code: '{Code}' Description: {Description}",
                consumeResult.Message.Value.Code,
                consumeResult.Message.Value.Description);

            if (consumeResult.IsRetryLimitExceeded())
            {
                // Verifica se o limite de tentativas de retry foi excedido para o fluxo de retry configurado.
                return;
            }

            if (consumeResult.Message.Value.Code == 0)
            {
                // Coloca a mensagem em retry conforme necessário. Invoca o .TryAgain() para reprocessamento.
                consumeResult.TryAgain();
                return;
            }

            if (consumeResult.Message.Value.Code < 0)
            {
                // Envia a mensagem diretamente para a Dead Letter Queue (DLQ), invocando o método .SkipRetryAndSendToDeadLetter.
                consumeResult.SkipRetryAndSendToDeadLetter();
                return;
            }

            // Opcional produzindo a mensagem consumida para outro destino
            await _producer.SendAsync(new MessageExampleProducer(
                consumeResult.Message.Value.Code,
                consumeResult.Message.Value.Description));

            // Simula tempo de processamento com um delay.
            await Task.Delay(15000);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar a mensagem.");

            // Se o retry estiver habilitado, lançar uma exceção fará com que a mensagem seja automaticamente enviada para o tópico de retry.
            // Não é necessário invocar o .TryAgain() manualmente.
            throw;
        }
    }
}
```

> :warning: **Nota Importante**: Ao processar uma mensagem em sistemas distribuídos, podem ocorrer
situações em que a mensagem não pode ser processada corretamente na primeira tentativa devido a várias
razões, incluindo falhas temporárias, problemas de rede, falhas de serviços dependentes ou 
violações de regras de negócio. Neste casos, é útil ter uma maneira de reenviar a mensagem para o 
processamento novamente, ao invés de simplesmente falhar e potencialmente perder a mensagem. 
A função `TryAgain()` permite lidar com esses cenários. Ao invocar `TryAgain()` para uma mensagem,
o sistema reconhecerá que a mensagem deve ser redirecionada para tentativas de processamento subsequentes.

> :warning: **Nota Importante**: `SkipRetryAndSendToDeadLetter()` é utilizado para mensagens 
que falham de forma irrecuperável. Ao invocá-lo, o processo de retry é ignorado e a mensagem é direcionada 
imediatamente para a Dead Letter Queue (DLQ), permitindo uma análise posterior sem afetar o fluxo normal de 
processamento e sem desperdiçar recursos em tentativas adicionais de reprocessamento.

> :warning: **Nota Importante** `IsRetryLimitExceeded()` permite identificar se uma
mensagem ultrapassou o limite de tentativas de reprocessamento, facilitando tratamentos específicos nas 
regras de negócio para fluxos de retry configurados.

## Recomendações
Certifique-se de sempre configurar ambos (produtor e consumidor) adequadamente.

## Licença
Este projeto está sob a licença Copyright © POC SA 2023.

## Teste Carga
Usuários simultâneos: 130

Duração: 1 minuto

Thresholds p(95): 500ms

Resultado:  
* http_req_duration..............: avg=304.64ms min=175.62ms max=974.51ms p(50)=280.22ms p(95)=473.05ms p(99)=673.22ms*

```bash
running (1m01.1s), 000/130 VUs, 5898 complete and 0 interrupted iterations
default ✓ [======================================] 130 VUs  1m0s

     data_received..................: 5.9 MB 97 kB/s
     data_sent......................: 830 kB 14 kB/s
     http_req_blocked...............: avg=25.96ms  min=0s       max=2.27s    p(50)=0s       p(95)=0s       p(99)=1.18s
     http_req_connecting............: avg=190.72µs min=0s       max=28.43ms  p(50)=0s       p(95)=0s       p(99)=10ms
   ✓ http_req_duration..............: avg=304.64ms min=175.62ms max=974.51ms p(50)=280.22ms p(95)=473.05ms p(99)=673.22ms
       { expected_response:true }...: avg=304.64ms min=175.62ms max=974.51ms p(50)=280.22ms p(95)=473.05ms p(99)=673.22ms
     http_req_failed................: 0.00%  ✓ 0         ✗ 5898
     http_req_receiving.............: avg=541.93µs min=0s       max=54.18ms  p(50)=0s       p(95)=2.4ms    p(99)=3.45ms
     http_req_sending...............: avg=159.3µs  min=0s       max=17ms     p(50)=0s       p(95)=819.77µs p(99)=1ms
     http_req_tls_handshaking.......: avg=25.36ms  min=0s       max=2.24s    p(50)=0s       p(95)=0s       p(99)=1.15s
     http_req_waiting...............: avg=303.94ms min=175.62ms max=973.51ms p(50)=279.33ms p(95)=471.8ms  p(99)=673.19ms
     http_reqs......................: 5898   96.538459/s
     iteration_duration.............: avg=1.33s    min=1.17s    max=3.69s    p(50)=1.28s    p(95)=1.51s    p(99)=2.68s
     iterations.....................: 5898   96.538459/s
     vus............................: 23     min=23      max=130
     vus_max........................: 130    min=130     max=130

```
 
 20/10/23 v1.0.0
 06/02/24 v1.1.0