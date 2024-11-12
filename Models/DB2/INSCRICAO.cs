using SiteSesc.Models.ApiPagamento;

namespace SiteSesc.Models.DB2
{
    public partial class INSCRICAO
    {
        public int CDUOP { get; set; }
        public int CDPROGRAMA { get; set; }
        public int SQMATRIC { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public Nullable<int> CDDESCONTO { get; set; }
        public int CDFONTEINF { get; set; }
        public Nullable<int> CDFORMATO { get; set; }
        public Nullable<int> CDPERFIL { get; set; }
        public short STINSCRI { get; set; }
        public Nullable<System.DateTime> DTPREINSCR { get; set; }
        public System.DateTime DTINSCRI { get; set; }
        public string LGINSCRI { get; set; }
        public Nullable<System.DateTime> DTPRIVENCT { get; set; }
        public short NUCOBRANCA { get; set; }
        public short CDUOPINSC { get; set; }
        public System.DateTime DTSTATUS { get; set; }
        public System.TimeSpan HRSTATUS { get; set; }
        public string LGSTATUS { get; set; }
        public short CDUOPSTAT { get; set; }
        public string DSSTATUS { get; set; }
        public Nullable<short> STCANCELAD { get; set; }
        public Nullable<short> CDCATEGORI { get; set; }

        public virtual CATEGORIA CATEGORIA { get; set; }
        public virtual CLIENTELA CLIENTELA { get; set; }
        public virtual PROGOCORR PROGOCORR { get; set; }
    }
}
