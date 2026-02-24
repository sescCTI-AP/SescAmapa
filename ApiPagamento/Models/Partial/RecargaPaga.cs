namespace PagamentoApi.Models.Partial
{
    public class RecargaPaga
    {
        public int Sequencia { get; set; }
        public string Habilitacao { get; set; }
        public string Cliente { get; set; }
        public double Valor { get; set; }
        public string Data { get; set; }
        public string Moeda { get; set; }
        public string Tid { get; set; }

    }
}