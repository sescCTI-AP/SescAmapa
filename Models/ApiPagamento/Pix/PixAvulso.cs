namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixAvulso
    {
        public int cduop { get; private set; }
        public int sqmatric { get; private set; }
        public int cartao { get; private set; }
        public PixCriar Pix { get; private set; }

        public PixAvulso(decimal? valor, string cpfPagador, string nomePagador)
        {
            var pixDevedor = new PixDevedor(cpfPagador, nomePagador);
            var pixValor = new PixValor((decimal)valor);
            var pixCriar = new PixCriar(pixDevedor, pixValor, "PIX - SESC");

            cduop = 0;
            sqmatric = 0;
            cartao = 0;
            Pix = pixCriar;
        }
    }
}
    