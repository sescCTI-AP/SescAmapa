using System;

namespace PagamentoApi.Models
{
    public class PagamentoCielo
    {
        public int ID { get; set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANC { get; set; }
        public string MERCHANTORDER { get; set; }
        public string CARDNUMBER { get; set; }
        public string BRAND { get; set; }
        public string PROOFOFSALE { get; set; }
        public string TID { get; set; }
        public string AUTHORIZATIONCODE { get; set; }
        public string PAYMENTID { get; set; }
        public string TIPO { get; set; }
        public decimal VALOR { get; set; }
        public int PARCELAS { get; set; }
        public DateTime DTOPERACAO { get; set; }
        public int CAIXA { get; set; }
        public int CDPESSOA { get; set; }
        public int SQDEPRET { get; set; }
        public int CANCELADO { get; set; }

    }
}