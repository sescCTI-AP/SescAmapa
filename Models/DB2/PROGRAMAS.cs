namespace SiteSesc.Models.DB2
{
    public partial class PROGRAMAS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROGRAMAS()
        {
            this.CONFPROG = new HashSet<CONFPROG>();
            this.PROGRAMAS1 = new HashSet<PROGRAMAS>();
        }

        public int CDPROGRAMA { get; set; }
        public string NMPROGRAMA { get; set; }
        public string TECONTEUDO { get; set; }
        public short VBINSCRI { get; set; }
        public string DSDURACAO { get; set; }
        public string DSPERIODO { get; set; }
        public Nullable<int> CDPROGSUP { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public short STATUS { get; set; }
        public int CDUOP { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFPROG> CONFPROG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROGRAMAS> PROGRAMAS1 { get; set; }
        public virtual PROGRAMAS PROGRAMAS2 { get; set; }
    }
}
