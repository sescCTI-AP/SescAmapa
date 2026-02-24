namespace PagamentoApi.Models.BB
{
    public class Boleto
    {
        public string numeroBoletoBB { get; set; }
        public string dataRegistro { get; set; }
        public string dataVencimento { get; set; }
        public double valorOriginal { get; set; }
        public int carteiraConvenio { get; set; }
        public int variacaoCarteiraConvenio { get; set; }
        public string estadoTituloCobranca { get; set; }
        public int contrato { get; set; }
        public string dataMovimento { get; set; }
        public double valorAtual { get; set; }
        public double valorPago { get; set; }
        public string dataCredito { get; set; }

    }
}