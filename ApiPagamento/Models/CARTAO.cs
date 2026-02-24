using System;
using System.Collections.Generic;

namespace PagamentoApi.Models
{

    public partial class CARTAO
    {
        public CARTAO()
        {
            CLIENTELA = new HashSet<CLIENTELA>();
            HSTMOVCART = new HashSet<HSTMOVCART>();
            SALDOCARTAO = new HashSet<SALDOCARTAO>();
        }

        public int NUMCARTAO { get; set; }
        public short TPCONTRL { get; set; }
        public string PSWCART { get; set; }
        public DateTime DTATU { get; set; }
        public TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public DateTime DTCADASTRO { get; set; }
        public string CDBARRA { get; set; }
        public string NUCHIP { get; set; }
        public short TPCARTAO { get; set; }
        public short STCARTAO { get; set; }

        public virtual ICollection<CLIENTELA> CLIENTELA { get; set; }

        public virtual ICollection<HSTMOVCART> HSTMOVCART { get; set; }

        public virtual ICollection<SALDOCARTAO> SALDOCARTAO { get; set; }
    }
}