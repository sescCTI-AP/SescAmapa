using System;
using System.Collections.Generic;

namespace PagamentoApi.Models
{

    public partial class SALDOCARTAO
    {
        public int NUMCARTAO { get; set; }
        public int CDPRODUTO { get; set; }
        public decimal SLDQTCART { get; set; }
        public decimal SLDQTBLOQ { get; set; }
        public decimal SLDVLCART { get; set; }
        public decimal SLDVLBLOQ { get; set; }

        public virtual PRODUTOPDV PRODUTOPDV { get; set; }
        public virtual CARTAO CARTAO { get; set; }
    }
}