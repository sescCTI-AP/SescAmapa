using System;
using System.Collections.Generic;

namespace PagamentoApi.Models
{

    public partial class PRODUTOPDV
    {

        public int CDPRODUTO { get; set; }
        public string DSPRODUTO { get; set; }
        public DateTime DTATU { get; set; }
        public short TPCTRLSALD { get; set; }
        public TimeSpan HRATU { get; set; }
        public short TPACAOSALD { get; set; }
        public string LGATU { get; set; }
        public short STPRODUTO { get; set; }
        public DateTime? DTCANCELAD { get; set; }
        public string CDBARRA { get; set; }
        public short VBQUILO { get; set; }
        public int CDGRUPOPDV { get; set; }
        public int CDSUBGRPDV { get; set; }
        public short VBRECAGR { get; set; }
        public string CDUNIDADE { get; set; }
        public short TPCOMPROD { get; set; }
        public short VBPREPARO { get; set; }
        public virtual ICollection<HSTMOVCART> HSTMOVCART { get; set; }
        public virtual ICollection<SALDOCARTAO> SALDOCARTAO { get; set; }
    }
}