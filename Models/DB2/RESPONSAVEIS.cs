namespace SiteSesc.Models.DB2
{
    public partial class RESPONSAVEIS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RESPONSAVEIS()
        {
            this.RESPCLI = new HashSet<RESPCLI>();
        }

        public short VBATIVO { get; set; }
        public string NUCPF { get; set; }
        public string NUREGGERAL { get; set; }
        public System.DateTime DTEMIRG { get; set; }
        public string IDORGEMIRG { get; set; }
        public string NMRESPONSA { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RESPCLI> RESPCLI { get; set; }
    }
}
