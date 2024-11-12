using SiteSesc.Models.ApiPagamento;

namespace SiteSesc.Models.DB2
{
    public partial class RESPCLI
    {
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public string NUCPF { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }

        public virtual CLIENTELA CLIENTELA { get; set; }
        public virtual RESPONSAVEIS RESPONSAVEIS { get; set; }
    }
}
