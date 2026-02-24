using System;

namespace PagamentoApi.Models.Cielo
{
    public class Transacao
    {
        public string MerchantOrderId { get; set; }
        public Payment Payment { get; set; }
        public Cliente Customer { get; set; }
        public CobrancaValor CobrancaValor { get; set; }

        public bool IsHorarioDeGeracaoTransacao()
        {
            return (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 30) ? false : true;

        }
    }
}