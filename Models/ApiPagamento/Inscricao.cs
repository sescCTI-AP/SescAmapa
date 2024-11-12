using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class Inscricao
    {
        public int CDUOP { get; set; }

        public int SQMATRIC { get; set; }

        public int CDPROGRAMA { get; set; }

        public int CDCONFIG { get; set; }

        public int SQOCORRENC { get; set; }

        public int? CDFORMATO { get; set; }

        [NotMapped]
        public string cdelement => $"{CDPROGRAMA.ToString().PadLeft(8, '0')}{CDCONFIG.ToString().PadLeft(8, '0')}{SQOCORRENC.ToString().PadLeft(8, '0')}";
    }
}
