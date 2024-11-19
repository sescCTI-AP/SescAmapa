namespace SiteSesc.Models.ApiPagamentoV2
{
    public record RecargaPixRequest
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public int NumCartao { get; set; }
        public int Sqmatric { get; set; }
        public int Cduop { get; set; }
        public decimal Valor { get; set; }
        public string DescricaoPagamento { get; set; }
    }
}
