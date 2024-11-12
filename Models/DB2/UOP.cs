namespace SiteSesc.Models.DB2
{
    public partial class UOP
    {
        public int Id { get; protected set; }
        public int CDUOP { get; set; }
        public string NMUOP { get; set; }
        public short CDADMIN { get; set; }
        public string NUCGCUOP { get; set; }
        public TimeSpan HRATU { get; set; }
        public DateTime DTATU { get; set; }
        public string LGATU { get; set; }
        public string NUCGCANT { get; set; }
        public short VBDR { get; set; }
        public short VBCOLFER { get; set; }
        public int? NUELEM_AR { get; set; }
        public string AAMODA_AR { get; set; }
        public int? CDMODA_AR { get; set; }
        public short STUOP { get; set; }
        public short STREALIZAMATRICULA { get; set; }

        public virtual ICollection<COBRANCA> COBRANCA { get; set; }

        public virtual ICollection<PROGRAMAS> PROGRAMAS { get; set; }
    }
}
