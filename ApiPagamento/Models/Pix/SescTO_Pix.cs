using System;

namespace PagamentoApi.Models.Pix
{
    public class SescTO_Pix
    {
        public int ID { get; set; }
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public string IDCLASSE { get; set; }
        public DateTime CRIACAO { get; set; }
        public int EXPIRACAO { get; set; }
        public string TXID { get; set; }
        public string QRCODE { get; set; }
        public string LOCATION { get; set; }
        public decimal VALOR_ORIGINAL { get; set; }
        public decimal VALOR_COBRANCA { get; set; }
        public decimal JUROS { get; set; }
        public decimal MULTA { get; set; }
        public decimal DESCONTO { get; set; }
        public string CHAVE { get; set; }
        public string STATUS { get; set; }
        public string SOLICITACAO { get; set; }
        public int PRODUCAO { get; set; }
        public SescTO_Pix_Transacoes[] TRANSACOES { get; set; }
        public int TIPO { get; set; }
    }
}