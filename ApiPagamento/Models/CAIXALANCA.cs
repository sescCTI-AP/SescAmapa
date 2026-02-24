using System;

namespace PagamentoApi.Models
{
    public class CAIXALANCA
    {
        public int CDPESSOA { get; set; }
        public int SQCAIXA { get; set; }
        public int SQLANCAMEN { get; set; }
        public short TPLANCAMEN { get; set; }
        public string IDUSRLANCA { get; set; }
        public DateTime DTLANCAMEN { get; set; }
        public TimeSpan HRLANCAMEN { get; set; }
        public string DSLANCAMEN { get; set; }
        public decimal VLLANCAMEN { get; set; }
        public short STLANCAMEN { get; set; }
        public string DSSTATUS { get; set; }
    }
}