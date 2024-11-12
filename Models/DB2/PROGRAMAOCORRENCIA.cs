using SiteSesc.Models.ApiPagamento.Relatorios;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.DB2
{
    public class PROGRAMAOCORRENCIA
    {
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public string DSUSUARIO { get; set; }
        public DateTime? DTLINSCRI { get; set; }
        public DateTime? DTUINSCRI { get; set; }
        public DateTime? DTINIOCORR { get; set; }
        public DateTime? DTFIMOCORR { get; set; }
        public int NUVAGAS { get; set; }
        public int NUVAGASOCP { get; set; }
        public int AAMODA { get; set; }
        public int CDUOPCAD { get; set; }
        public int CDMAPA { get; set; }
        public string DSSUBMODAL { get; set; }

        [NotMapped]
        public string cdelement => $"{CDPROGRAMA.ToString().PadLeft(8, '0')}{CDCONFIG.ToString().PadLeft(8, '0')}{SQOCORRENC.ToString().PadLeft(8, '0')}";
    }
}
