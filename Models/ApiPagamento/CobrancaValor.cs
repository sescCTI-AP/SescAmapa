namespace SiteSesc.Models.ApiPagamento
{
    public class CobrancaValor
    {
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQCOBRANCA { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal JurosMora { get; set; }
        public decimal Multa { get; set; }
        public decimal OutrosRecebimentos { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal DescontoConcedido { get; set; }
        public string Atividade { get; set; }
        public DateTime Vencimento { get; set; }
    }
}
