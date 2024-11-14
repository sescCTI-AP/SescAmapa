namespace SiteSesc.Models.ApiPagamentoV2
{
    public abstract record CobrancaRequest
    {
        public string Cpf { get; set; } = null!;
        public int NumCartao { get; set; }
        public int SqMatric { get; set; }
        public int CdUop { get; set; }
        public string CdElement { get; set; } = null!;
        public int SqCobranca { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorAcresimo { get; set; }
        public decimal ValorDesconto { get; set; }
    }
}
