using System.Collections.Generic;

namespace PagamentoApi.Models.Cielo
{
    public class StatusTransacao
    {
        public string Descricao { get; set; }
        public int Codigo { get; set; }

        public static List<StatusTransacao> GetListaStatusTransacao()
        {
            return new List<StatusTransacao>
            {
                new StatusTransacao
                {
                    Descricao = "NotFinished - Aguardando atualização de status",
                    Codigo = 0
                },
                new StatusTransacao
                {
                    Descricao = "Authorized - Pagamento apto a ser capturado ou definido como pago",
                    Codigo = 1
                },
                new StatusTransacao
                {
                    Descricao = "PaymentConfirmed - Pagamento confirmado e finalizado",
                    Codigo = 2
                },
                new StatusTransacao
                {
                    Descricao = "Denied - Pagamento negado por Autorizador",
                    Codigo = 3
                },
                new StatusTransacao
                {
                    Descricao = "Voided - Pagamento cancelado",
                    Codigo = 10
                },
                new StatusTransacao
                {
                    Descricao = "Refunded - Pagamento cancelado após 23:59 do dia de autorização",
                    Codigo = 11
                },
                new StatusTransacao
                {
                    Descricao = "Pending - Aguardando Status de instituição financeira",
                    Codigo = 12
                },
                new StatusTransacao
                {
                    Descricao = "Aborted - Pagamento cancelado por falha no processamento ou por ação do AF",
                    Codigo = 13
                },
                new StatusTransacao
                {
                    Descricao = "Scheduled - Recorrência agendada",
                    Codigo = 20
                },
            };
        }
    }

    public class StatusRetornoCartaoCredito
    {
        public string Descricao { get; set; }
        public int Codigo { get; set; }

        public static List<StatusRetornoCartaoCredito> GetListaRetornoCartao()
        {
            return new List<StatusRetornoCartaoCredito>
            {
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Autorizado - Operação realizada com sucesso",
                    Codigo = 4
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Autorizado - Operação realizada com sucesso",
                    Codigo = 6
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado",
                    Codigo = 05
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado - Cartão Expirado",
                    Codigo = 57
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado - Cartão Bloqueado",
                    Codigo = 78
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado - Time Out",
                    Codigo = 99
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado - Cartão Cancelado",
                    Codigo = 77
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Não Autorizado - Problemas com o Cartão de Crédito",
                    Codigo = 70
                },
                new StatusRetornoCartaoCredito
                {
                    Descricao = "Autorização Aleatória - Operation Successful / Time Out",
                    Codigo = 99
                }
            };
        }
    }
}
