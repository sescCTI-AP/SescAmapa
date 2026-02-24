using System;

namespace PagamentoApi.Models
{
    public class CXDEPRETPDV
    {
        public short TPDEPRET { get; set; }
        public int CDPESSOA { get; set; }
        public int SQCAIXA { get; set; }
        public short SQDEPRET { get; set; }
        public decimal VLDEPRET { get; set; }
        public DateTime DTDEPRET { get; set; }
        public TimeSpan HRDEPRET { get; set; }
        public int NUMCARTAO { get; set; }
        public short STDEPRET { get; set; }
        public decimal VLENCARGOS { get; set; }
        public int CDMOEDAPGT { get; set; }

    }
}