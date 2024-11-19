namespace SiteSesc.Models.ApiPagamentoV2
{
    public record CobrancaCartaoCieloRequest : CobrancaRequest
    {
        public CartaoCieloRequest CartaoCielo { get; set; } = null!;
    }
}
