using System.Collections.Generic;

namespace PagamentoApi.Models.Cielo
{
    public class ReturnPayments
    {
        public int ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public List<PaymentPartial> Payments { get; set; }
    }
}