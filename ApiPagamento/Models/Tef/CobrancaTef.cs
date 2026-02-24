namespace PagamentoApi.Models.Tef
{
    public class CobrancaTef
    {
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public decimal Valor { get; set; }

        public TransacaoTotem Transacao { get; set; }
        //public Receipt Receipt { get; set; }
    }
}