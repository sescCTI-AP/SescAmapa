using System;

namespace PagamentoApi.Models.Tef
{
    public class TransacaoTotem
    {
        public string CDELEMENT { get; set; }
        public int? SQCOBRANCA { get; set; }
        public string IDCLASSE { get; set; }
        public string NOMEPAGADOR { get; set; }
        public string CARDNUMBER { get; set; }
        public string CUPOM { get; set; }
        public string IDTRANSACAO { get; set; }
        public string TIPOPAGAMENTO { get; set; }
        public decimal VALOR { get; set; }
        public int CDPESSOA { get; set; }
        public DateTime DATATRANSACAO { get; set; }
        public string COMPROVANTECLIENTE { get; set; }
        public string COMPROVANTELOJA { get; set; }
        public int? SQDEPRET { get; set; }
        public int? CAIXA { get; set; }
        public bool CANCELADO { get; set; }
        public int? SQCUPOM { get; set; }
        public string CODTOTEM { get; set; }
        public string CPF { get; set; }
    }
}
