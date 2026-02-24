using System;

namespace PagamentoApi.Models.Partial
{
    public class CobrancaCliente
    {
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public short NUDV { get; set; }
        public string NMCLIENTE { get; set; }
        public DateTime DTVENCTO { get; set; }
        public string DSCOBRANCA { get; set; }
        public decimal VLCOBRADO { get; set; }
        public int CDUOPREC { get; set; }
        public string NMUOP { get; set; }
        public string MATFORMAT { get; set; }
        public string DSMAPA { get; set; }
        public string CDMAPA { get; set; }
        public string DSCONTATO { get; set; }
    }
}
