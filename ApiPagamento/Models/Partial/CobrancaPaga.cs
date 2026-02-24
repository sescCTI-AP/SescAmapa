namespace PagamentoApi.Models.Partial
{
    public class CobrancaPaga
    {
        public string Habilitacao { get; set; }
        public string Cliente { get; set; }
        public string Atividade { get; set; }
        public string Vencimento { get; set; }
        public double Valor { get; set; }
        public double Juros { get; set; }
        public double Acrescimo { get; set; }
        public double Desconto { get; set; }
        public string Moeda { get; set; }
        public string Tid { get; set; }
    }
}