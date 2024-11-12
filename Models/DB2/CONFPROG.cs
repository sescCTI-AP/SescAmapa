using Microsoft.VisualStudio.Web.CodeGeneration.Design;

namespace SiteSesc.Models.DB2
{
    public partial class CONFPROG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONFPROG()
        {
            this.PROGOCORR = new HashSet<PROGOCORR>();
        }

        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public string NMCONFIG { get; set; }
        public short CDUNIDTEMP { get; set; }
        public Nullable<int> CDFORMATO { get; set; }
        public Nullable<decimal> VLDURACAO { get; set; }
        public short VBPREINSCR { get; set; }
        public Nullable<short> STPASSAGEM { get; set; }
        public short VBREJEITA { get; set; }
        public short VBTRANINSC { get; set; }
        public Nullable<decimal> VLDURTRANC { get; set; }
        public Nullable<short> STREINGREC { get; set; }
        public string TECONTEUDO { get; set; }
        public short VBCLIFORA { get; set; }
        public short VBABERTA { get; set; }
        public Nullable<short> DDINICINSC { get; set; }
        public Nullable<short> DDFIMINSC { get; set; }
        public Nullable<short> DDRENOVACAO { get; set; }
        public string IDUSRRESP { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public short VBRODIZIO { get; set; }
        public Nullable<int> CURSO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROGOCORR> PROGOCORR { get; set; }
        public virtual PROGRAMAS PROGRAMAS { get; set; }
    }
}
