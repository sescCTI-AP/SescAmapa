using System;

namespace PagamentoApi.Models
{
    public class VALORPARC
    {
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public int CDFORMATO { get; set; }
        public int CDPARCELA { get; set; }
        public int CDPERFIL { get; set; }
        public decimal VLPARCELA { get; set; }
        public DateTime DTVENCTO { get; set; }
        public int TPPARCELA { get; set; }
        public int CDCATEGORI { get; set; }

    }
}