using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class HSTMOVCART
    {
        public int CDPRODUTO { get; set; }
        //public System.DateTime HRMOVIMENT { get; set; }
        public string HORAMOV { get; set; }
        public decimal VLPRODMOV { get; set; }
        public System.DateTime DTMOVIMENT { get; set; }
        public string DSPRODUTO { get; set; }

        [NotMapped]
        public string classMov => VLPRODMOV > 0 ? "tp-entrada" : "tp-saida";
    }
}
