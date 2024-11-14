namespace SiteSesc.Models.ApiPagamentoV2
{
    public record PixRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public decimal Valor { get; set; }
        public string DescricaoPagamento { get; set; } = string.Empty;
    }
}
