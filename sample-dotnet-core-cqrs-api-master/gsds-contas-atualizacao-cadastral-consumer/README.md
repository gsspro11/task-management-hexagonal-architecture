<h1 align="center">GCNT</h1> 
<h2 align="center">Microserviço: gsds-contas-atualizacao-cadastral-consumer</h1> 

<p align="center">
<img src="https://img.shields.io/badge/8.0.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white" />
<img src="https://img.shields.io/static/v1?label=C%20Sharp&message=10.0.0&color=blue&style=for-the-badge&logo=c-sharp"/>
<img src="https://img.shields.io/static/v1?label=nuget&message=Dependencies&color=blue&style=for-the-badge&logo=NUGET"/>
<img src="https://img.shields.io/badge/Oracle-316192?style=for-the-badge&logo=oracle&logoColor=white" />
<img src="https://img.shields.io/badge/Kafka-D9281A?style=for-the-badge&logo=apachekafka&logoColor=white"/>
<br>
<img src="https://img.shields.io/badge/Redis-D9281A?style=for-the-badge&logo=redis&logoColor=white"/>
<img src="https://img.shields.io/badge/Amazon_AWS-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white" />
<img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
</p>

<br><br>
<img src="https://img.shields.io/badge/STATUS-EM%20DESENVOLVIMENTO-GREEN"/>

<br>

## Tópicos 

<p align="center">
<ul>
  <li><a href="#sobre-o-projeto">Sobre o Projeto</a></li>
  <li><a href="#tecnologias">Tecnologias</a></li> 
  <li><a href="#arquitetura">Arquitetura</a></li>
  <li><a href="#pré-requisitos">Pré-requisitos</a></li>
  <li><a href="#como-executar-o-projeto">Como executar o projeto</a></li>
  <li><a href="#build-e-release-e-monitoramento">Build, Release e Monitoramento</a></li>
  <li><a href="#variáveis-de-ambiente-necessárias">Variáveis de ambiente necessárias</a></li>
  <li><a href="#funcionalidades">Funcionalidades</a></li>
  <li><a href="#casos-de-uso">Casos de uso</a></li>
</ul>  
</p>

## Sobre o projeto 

<p align="justify">
  Serviço consumidor que realiza a tentativa de bloqueio de contas correntes de clientes com situação cadastral de óbito.

  Tópicos consumidos:

  - "CADU-NOTIFICA-ALTERACAO-STATUS-CADASTRAL"
</p>

## Tecnologias

- .NET 8.0
- C# 10
- Oracle
- Kafka
- XUnit

## Arquitetura

Visão geral:

![Diagrama de arquitetura](Docs/ArquiteturaContasAtualizacaoCadastralConsumer.png)

Arquitetura utilizada no projeto é baseada no template disponibilizado pelo POC: [<b>Template arquitetura hexagonal POC</b>](https://orangebox.poc.cloud/docs/default/component/poc-doc/tutorial/desenvolvimento-local/baixar-template-dotnet/).

## Pré-requisitos

 [.Net 8.0](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
 
 [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)

 [Nuget](https://www.nuget.org)

## Como executar o projeto

No terminal, clone o projeto: 

```Bash
git clone 

cd gsds-contas-atualizacao-cadastral-consumer

nuget install # Baixe as dependencias

dotnet test # Realiza os testes unitários e demais testes caso existam

#Projeto: Poc.ContasAtualizacaoCadastralConsumer.ConsumerService

dotnet run # Executa a aplicação 

```
## Build e release e monitoramento

> Pipeline:  <br>
Dynatrace HML: 

## Variáveis de ambiente necessárias

- Conexão com o banco de dados Oracle
```json
  "ConnectionStrings": {
    "ExadataConnection": "Server="";Port=3306;userid="";pwd="";database="";Max Pool Size="";Connection Lifetime="";Min Pool Size="";Pooling="";Connection Timeout="";"
  }
```
- Configuração específica do microserviço
```json
{
"ContasAtualizacaoCadastralConsumer.ConsumerService": {
  "Name": "gsds-contas-atualizacao-cadastral-consumer",
  "ApiConsumptionTimeoutMs": 15000,
  "ProcessTimeoutMs": 45000
}
```
- Configuração das variáveis do Kafka
```json
{
  "PocKafkaBrokerSettings": {
    "BrokerCorp": {
      "BootstrapServer": "servidor do Kafka",
      "Topics": {
        "CADU-NOTIFICA-ALTERACAO-STATUS-CADASTRAL": {
          "MaxConcurrentMessages": 100,
          "MaxInFlight": 5,
          "MessageSendMaxRetries": 3,
          "RetryLimit": 1
        }
      }
    }
  }
}
 
```

## Funcionalidades

- Consumer Service:
  - [x] Tentativa de bloqueio de conta

## Casos de Uso

Ao executar o projeto "Poc.ContasAtualizacaoCadastralConsumer.ConsumerService" o consumer é iniciado e passa a receber as mensagens dos tópicos "CADU-NOTIFICA-ALTERACAO-STATUS-CADASTRAL".

Cada mensagem recebida contém os dados necessários para que a aplicação execute a tentativa de realização de um bloqueio de conta.

Exemplo de mensagem recebida: 
```json
{
  "Id": 3017497,
  "CpfCnpj": "35467738453",
  "DataHoraInclusao": "2024-06-06T00:00:00",
  "DataHoraAtualizacao": "2024-06-06T00:00:00",
  "ApiEndpoint": "https://gcp023-clientes-hml.cloudpoc.app.br/api/v1/Clientes/35467738453",
  "Schema": {
      "Cliente": {
          "Propriedades": {
              "SituacaoCadastral": {
                  "Type": "Enumeration",
                  "Value": {
                      "Name": "CANCELADA_POR_OBITO_SEM_ESPOLIO",
                      "Id": 3
                  }
              }
          }
      }
  },
  "TipoEvento": "CADU-NOTIFICA-ALTERACAO-STATUS-CADASTRAL"
}
```
<br>
Com a mensagem recebida, o serviço dá início ao processamento: 

- Consulta na api <b>gsds-contas-pessoas-consultas</b> para verificar a existência de contas para o CPF/CPNJ recebido.
- Caso não existam contas vinculadas ao CPF/CPNJ encerra o processamento.
- Caso existam contas vinculadas ao CPF/CNPJ realiza a tentativa de bloqueio da conta no sistema.


Após a realização da tentativa deste bloqueio no IMP001 podemos considerar 3 tipos de retorno, que são: <b>Sucesso, Erro de negócio e Erro sistêmico.</b>