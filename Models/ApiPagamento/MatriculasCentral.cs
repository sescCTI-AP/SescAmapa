using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class MatriculasCentral
    {
        public int cduop { get; set; }
        public int cdprograma { get; set; }
        public int sqmatric { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrenc { get; set; }
        public int cdformato { get; set; }
        public int cdperfil { get; set; }
        public int stinscri { get; set; }
        public DateTime dtinscri { get; set; }
        public int cduopinsc { get; set; }
        public int? cdcategori { get; set; }
        public string stcancelad { get; set; }
        public ProgocorrApi progocorr { get; set; }

        [NotMapped]
        public string cdelement => $"{cdprograma.ToString().PadLeft(8, '0')}{cdconfig.ToString().PadLeft(8, '0')}{sqocorrenc.ToString().PadLeft(8, '0')}";

    }

    public class ProgocorrApi
    {
        public string dsusuario { get; set; }
        public DateTime dtiniocorr { get; set; }
        public DateTime dtfimocorr { get; set; }
        public int nuvagas { get; set; }
        public int? aamoda { get; set; }
    }
}
