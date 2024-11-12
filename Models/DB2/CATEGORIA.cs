using SiteSesc.Models.ApiPagamento;

namespace SiteSesc.Models.DB2
{
    public partial class CATEGORIA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIA()
        {
            this.CLIENTELA = new HashSet<CLIENTELA>();
            this.INSCRICAO = new HashSet<INSCRICAO>();
        }

        public short CDCATEGORI { get; set; }
        public string DSCATEGORI { get; set; }
        public short TPCATEGORI { get; set; }
        public System.DateTime DTATU { get; set; }
        public string CDIMPRESS { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public short VBCATSERV { get; set; }
        public short VBPDV { get; set; }
        public short VBATIVA { get; set; }
        public short VBEMPOBR { get; set; }
        public short IDCATEGORI { get; set; }
        public short VBCATCONV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTELA> CLIENTELA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INSCRICAO> INSCRICAO { get; set; }
    }
}
