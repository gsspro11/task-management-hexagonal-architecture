namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Adapters.Integrations.Queues.ContasAtualizacaoCadastralConsumerService
{
    public class ContasAtualizacaoCadastralMessage
    {
        public int Id { get; set; }
        public string CpfCnpj { get; set; }
        public DateTime DataHoraInclusao { get; set; }
        public DateTime DataHoraAtualizacao { get; set; }
        public string ApiEndpoint { get; set; }
        public Schema Schema { get; set; }
        public string TipoEvento { get; set; }
    }

    public class Schema
    {
        public Cliente Cliente { get; set; }
    }

    public class Cliente
    {
        public Propriedades Propriedades { get; set; }
    }

    public class Propriedades
    {
        public SituacaoCadastral SituacaoCadastral { get; set; }
    }

    public class SituacaoCadastral
    {
        public string Type { get; set; }
        public Value Value { get; set; }
    }

    public class Value
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
