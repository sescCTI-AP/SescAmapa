using System;

namespace PagamentoApi.Models
{
    public class HSTCANINSC
    {
        public int CDCANCELA { get; set; }
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }

        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public int SQCANCELA { get; set; }
        public DateTime DTATU { get; set; }
        public TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
    }
}