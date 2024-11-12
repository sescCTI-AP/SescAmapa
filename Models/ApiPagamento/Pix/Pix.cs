using System;

namespace SiteSesc.Models.ApiPagamento.Pix{
    public class Pix
    {
        public string endToEndId { get; set; }

        public string txid { get; set; }

        public decimal valor { get; set; }


        public DateTime horario { get; set; }
        public PixDevedor pagador { get; set; }

        public string infoPagador { get; set; }

        public PixDevolucao[] devolucoes { get; set; }
    }
}
