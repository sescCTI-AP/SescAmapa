namespace PagamentoApi.Models
{
    using System.Collections.Generic;
    using System;

    public partial class DESCACRES
    {
        public int Id { get; protected set; }
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public int CDFORMATO { get; set; }
        public short SQLANCAMEN { get; set; }
        public Nullable<int> CDPERFIL { get; set; }
        public Nullable<int> DDLIMITE { get; set; }
        public short RFDIASLMT { get; set; }
        public short TPLANCAMEN { get; set; }
        public short TPVALOR { get; set; }
        public decimal VLDESCACRE { get; set; }
        public System.DateTime DTATU { get; set; }
        public System.TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public string REFERENCIA { get; set; }
        public string LANCAMENTO { get; set; }
        public string TIPOVALOR { get; set; }

        public virtual PROGOCORR PROGOCORR { get; set; }
    }
}