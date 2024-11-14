namespace SiteSesc.Models.ApiPagamentoV2
{
    public record CobrancaPixRequest : CobrancaRequest
    {
        public PixRequest Pix { get; set; } = null!;
    }
}
