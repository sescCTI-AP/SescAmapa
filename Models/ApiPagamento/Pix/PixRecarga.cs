namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixRecarga
    {
        public int cduop { get; private set; }
        public int sqmatric { get; private set; }
        public int cartao { get; private set; }
        public PixCriar Pix { get; private set; }

        //public PixRecarga(int cduop, int sqmatric, int cartao, PixCriar pix)
        //{
        //    this.cduop = cduop;
        //    this.sqmatric = sqmatric;
        //    this.cartao = cartao;
        //    Pix = pix;
        //}

        public PixRecarga(decimal? valorRecarga, string cpfPagador, string nomePagador, ClienteCentral cliente)
        {
            var pixDevedor = new PixDevedor(cpfPagador, nomePagador);
            var pixValor = new PixValor((decimal)valorRecarga);
            var pixCriar = new PixCriar(pixDevedor, pixValor, "RECARGA PIX - SITE SESC");

            cduop = cliente.Cduop;
            sqmatric = cliente.Sqmatric;
            cartao = cliente.Numcartao;
            Pix = pixCriar;
        }
    }
}