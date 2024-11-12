using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixCriado
    {
        public PixCalendario calendario { get; set; }
        public string status { get; set; }
        public string txid { get; set; }
        public string textoImagemQRcode { get; set; }
        public string revisao { get; set; }
        public string location { get; set; }
        public PixDevedor devedor { get; set; }
        public PixValor valor { get; set; }

        [MaxLength(77)]
        public string chave { get; set; }

        [MaxLength(140)]
        public string solicitacaoPagador { get; set; }

        public List<PixInfoAdicionais> infoAdicionais { get; set; }

        public Pix[] Pix { get; set; }
    }
}
