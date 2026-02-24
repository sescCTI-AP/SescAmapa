namespace PagamentoApi.Models.BB
{
    public class BoletoRetorno
    {
         public string numero { get; set; } 
        public int numeroCarteira { get; set; } 
        public int numeroVariacaoCarteira { get; set; } 
        public int codigoCliente { get; set; } 
        public string linhaDigitavel { get; set; } 
        public string codigoBarraNumerico { get; set; } 
        public int numeroContratoCobranca { get; set; } 
        public Beneficiario beneficiario { get; set; } 
        public QrCode QrCode { get; set; }
    }
}