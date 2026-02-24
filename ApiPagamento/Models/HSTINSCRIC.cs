using System;

namespace PagamentoApi.Models
{
    public class HSTINSCRIC
    {
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public int SQHISTORIC { get; set; }
        public short STREGISTRO { get; set; }
        public string DSACONTEC { get; set; }
        public DateTime DTREGISTRO { get; set; }
        public TimeSpan HRREGISTRO { get; set; }
        public string LGRESPONSA { get; set; }
        public int CDUOPREG { get; set; }

    }
}