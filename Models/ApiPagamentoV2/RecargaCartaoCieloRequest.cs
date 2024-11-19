namespace SiteSesc.Models.ApiPagamentoV2
{
    public record RecargaCartaoCieloRequest
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public int NumCartao { get; set; }
        public int Sqmatric { get; set; }
        public int Cduop { get; set; }
        public decimal Valor { get; set; }
        public int Parcela { get; set; }
        public string NumeroCartaoCredito { get; set; }
        public string DataExpiracaoCartaoCredito { get; set; }
        public string CodigoSegurancaCartaoCredito { get; set; }
        public string BandeiraCartaoCredito { get; set; }
    }
}
