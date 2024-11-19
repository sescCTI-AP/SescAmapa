namespace SiteSesc.Models.ApiPagamentoV2
{
    public record CartaoCieloRequest
    {
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public decimal Valor { get; set; }
        public int Parcela { get; set; } = 1;
        public string NumeroCartao { get; set; } = null!;
        public string DataExpiracao { get; set; } = null!;
        public string CodigoSeguranca { get; set; } = null!;
        public string Bandeira { get; set; } = null!;
    }
}
