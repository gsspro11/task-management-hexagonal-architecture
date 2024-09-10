using System.ComponentModel;

namespace Poc.ContasAtualizacaoCadastralConsumer.Domain.Enums
{
    public enum RetornoProcessamento
    {
        [Description("Erro não definido")]
        ERRO_NÂO_DEFINIDO,

        [Description("Processado com sucesso")]
        SUCESSO,

        [Description("Informação não encontrada")]
        ERRO_INFORMACAO_NAO_ENCONTRADA,

        [Description("Login e/ou Senha inválido(s)")]
        ERRO_NEGOCIO_LOGIN_SENHA_INVALIDOS,

        [Description("Processado com erro")]
        ERRO_SISTEMA,

        [Description("Conta e/ou agência inválida(s)")]
        ERRO_CONTA_AGENCIA_INVALIDA,

        [Description("Cadrtão de crédito inválido")]
        ERRO_CARTAO_CREDITO_INVALIDO,

        [Description("Processado com exceção")]
        ERRO_DESCONHECIDO,

        [Description("Erro regra de negócio")]
        ERRO_REGRA_NEGOCIO,

        [Description("Sem descrição de erro")]
        ERRO_SEM_DESCRICAO = 572
    }
}