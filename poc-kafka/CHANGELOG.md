# Changelog

### Poc.Kafka 8.1.1
- Alteração da visibilidade dos serializers utilizados no Kafka

### Poc.Kafka 8.1.0

#### Break Change!
1º Nova classe de configuração para mapeamento Parameter Store.
- Possibilidade de inscrição em múltiplos tópicos.
- Capacidade de assinar uma partição específica.
- Separação entre produtor  e consumidor para maior clareza semântica.

2º Aprimoramento do health check, agora incluindo validação dos tópicos dependentes.

3º Alterações na configuração Cluster IoC
- Melhorias de Design para Facilitação do Uso.
- Suporte a múltiplo clusters.
- Criação Dinâmica de Tópicos para Maior Portabilidade.
- Configuração Personalizada do Schema Registry.

4º Alterações na configuração Consumer IoC
- Opções Ampliadas de Configuração.
- Customizações de Serialização.
- Suporte para Assinatura de Partições Específicas

5º Alterações na configuração Producer IoC
- Opções Ampliadas de Configuração

6º Utilização Consumer
- Utilização via keyed service permite a múltiplos consumers operar com a mesma chave de partição e valor.

7º Utilização Producer
- Utilização via keyed service permite a múltiplos producers operar com a mesma chave de partição e valor.

8º Novos métodos e sobrecargas facilitam a publicação de mensagens individuais
- Método "SendAsync", com nova sobrecarga, permite enviar uma mensagem Kafka.
- Novos métodos síncronos "Send" permitem especificar um deliveryHandle.

9º Novos métodos e sobrecargas facilitam a publicação de lotes de mensagens
- Novos métodos síncronos "SendBatchAsync" permitem enviar um lote de mensagens e informar quais foram entregues com sucesso e quais falharam.
- Novos métodos síncronos "SendBatch" permitem enviar um lote de mensagens e indicar as entregas bem-sucedidas e as falhas.
- Novos métodos síncronos "SendBatchAtomic" oferecem a opção de enviar um lote com garantia de atomicidade, retornando um booleano que indica se a operação foi bem sucedida.

10º Garantia de ordem no commit de offset para uma partição específica.

11º Segregação implementada para o consumer online dedicado a retentativas (retry).

12º Expansão na cobertura de testes realizada.
- A cobertura dos testes foi aumentada para impressionantes 96,5%.

### Poc.Kafka 8.0.1
- Ajuste Helthcheck sem necessidade de consumir tópico para validação

### Poc.Kafka 8.0.0
- Migração .NET 8

### Poc.Kafka 6.2.3
- Adicionado método IsRetryLimitExceeded;
- Correção do problema de inicialização concorrente entre producer e consumer;
- Correção no MaxConcurrentMessages para funcionar conforme esperado aguardando o lote de threads para processar junto;
- Adicionando possibilidade de setar propriedades FetchWaitMaxMs, FetchMinBytes, FetchMaxBytes e MaxPartitionFetchBytes;
- Possibilitar a subscrição em múltiplos tópicos pelo consumer;
- Incluir no `PocConsumeResult` o `RetryLimit` definido na inicialização do consumer;
- Corrigir o log do consumer para verificar se mensagem nula;
- Adicionando propriedade RetryDelayMs para possibilitar definição de um delay entre retentativas.


### Poc.Kafka 6.0.1
- Correção na serialização e deserialização de Json
- Correção de logs na aplicação
- Adição de testes unitários


### Poc.Kafka 6.0.0
- Lançamento biblioteca para utilização do Kafka Gerenciado AWS