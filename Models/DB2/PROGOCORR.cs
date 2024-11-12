namespace SiteSesc.Models.DB2
{
    public partial class PROGOCORR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROGOCORR()
        {
            this.INSCRICAO = new HashSet<INSCRICAO>();
            this.DESCACRES = new HashSet<DESCACRES>();
        }

        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public string DSUSUARIO { get; set; }
        public Nullable<System.DateTime> DTLINSCRI { get; set; }
        public Nullable<System.DateTime> DTUINSCRI { get; set; }
        public Nullable<System.DateTime> DTLPREINSC { get; set; }
        public Nullable<System.DateTime> DTUPREINSC { get; set; }
        public Nullable<System.DateTime> DTLTRANC { get; set; }
        public Nullable<System.DateTime> DTUTRANC { get; set; }
        public Nullable<System.DateTime> DTINIOCORR { get; set; }
        public Nullable<System.DateTime> DTFIMOCORR { get; set; }
        public short VBINSCRUOP { get; set; }
        public string IDUSRRESP { get; set; }
        public int NUVAGAS { get; set; }
        public int NUVAGASOCP { get; set; }
        public int NUMINVOCUP { get; set; }
        public short VBOCORRAPR { get; set; }
        public int CDUOPCAD { get; set; }
        public Nullable<System.DateTime> DTAPROV { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public short DURAULA { get; set; }
        public decimal IDADEMIN { get; set; }
        public decimal IDADEMAX { get; set; }
        public short VBCANCELA { get; set; }
        public string AAMODA { get; set; }
        public Nullable<int> CDMODA { get; set; }
        public short VBBOLBAN { get; set; }
        public Nullable<System.DateTime> DTINIFER { get; set; }
        public Nullable<System.DateTime> DTFIMFER { get; set; }
        public string IDTURMASGP { get; set; }
        public string IDVARIAVELTIPO { get; set; }
        public Nullable<decimal> VLMEDIO { get; set; }

        public virtual CONFPROG CONFPROG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INSCRICAO> INSCRICAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DESCACRES> DESCACRES { get; set; }
    }
}
