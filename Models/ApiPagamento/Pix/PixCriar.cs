namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixCriar
    {
        public PixCalendario calendario { get; set; }
        public PixDevedor devedor { get; set; }
        public PixValor valor { get; set; }
        public PixInfoAdicionais infoAdicionais { get; set; }
        public string chave { get; set; }
        public string solicitacaoPagador { get; set; }

        public PixCriar(PixDevedor pixDevedor, PixValor pixValor, string solicitacaoPagador)
        {
            calendario = new PixCalendario(3600);
            devedor = pixDevedor;
            valor = pixValor;
            this.solicitacaoPagador = solicitacaoPagador;
        }
    }
}