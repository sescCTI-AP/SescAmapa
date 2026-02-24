using System;

namespace PagamentoApi.Models.Boleto
{
    public class SescTO_Boletos
    {
        public int ID { get; set; }
        public int SQMATRIC_CLIENTELA { get; set; }
        public int CDUOP_CLIENTELA { get; set; }
        public string NOSSO_NUMERO { get; set; }
        public long BASE_NOSSO_NUMERO { get; set; }
        public decimal VALOR_BOLETO { get; set; }
        public decimal VALOR_JUROS_MULTA { get; set; }
        public decimal VALOR_DESCONTO { get; set; }
        public DateTime DATA_VENCIMENTO { get; set; }
        public int CONVENIO { get; set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public int STATUS { get; set; }
        public DateTime DATAGERACAO { get; set; }
        public string CPF_RESPONSAVEL { get; set; }
        public string LINHA_DIGITAVEL { get; set; }
        public string CODIGO_BARRAS { get; set; }
        public string QRCODE_URL { get; set; }
        public string QRCODE_TXID { get; set; }
        public string QRCODE_EMV { get; set; }
        public DateTime DATAATUALIZACAO { get; set; }
        public int PRODUCAO { get; set; }

    }
}