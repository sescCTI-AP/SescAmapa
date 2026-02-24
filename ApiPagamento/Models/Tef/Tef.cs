namespace PagamentoApi.Models.Tef
{
    public class Tef
    {
        public string NsuSitef { get; set; }
        public string NsuAutorizador { get; set; }
        public string CodigoAutorizacao { get; set; }
        public string Bandeira { get; set; }
        public string BandeiraCodigo { get; set; }
        public string Modalidade { get; set; }
        public string Mdr { get; set; }
        public string Bin { get; set; }
        public string Embosso { get; set; }
        public string NomePortador { get; set; }
        public string Adquirente { get; set; }
        public int NumeroParcelas { get; set; }
        public string DataHora { get; set; }
        public string RedeAutorizadora { get; set; }
        public string RedeAutorizadoraCodigo { get; set; }
        public decimal ValorCredito { get; set; }
        public string CodigoOperadoraRecarga { get; set; }
        public string NomeOperadoraRecarga { get; set; }
        public string NumeroCelularRecarga { get; set; }
    }
}