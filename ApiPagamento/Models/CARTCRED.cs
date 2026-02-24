using System;

namespace PagamentoApi.Models
{
    /// <summary>
    /// Saldo total de recargas do cartão sesc
    /// </summary>
    public class CARTCRED
    {
        public int CDPRODUTO { get; set; }
        public int NUMCARTAO { get; set; }
        public decimal? QTDPRODCRE { get; set; }
        public decimal? VALPRODCRE { get; set; }
        public decimal? QTDPRODBLO { get; set; }
        public decimal? VALPRODBLO { get; set; }
        public short VBATIVO { get; set; }
        public DateTime DTATU { get; set; }
        public TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
    }
}