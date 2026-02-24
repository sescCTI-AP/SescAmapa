using System;

namespace PagamentoApi.Models.Partial
{
    public class MovCartao
    {
        private TimeSpan _hrmoviment;
        public DateTime DTMOVIMENT { get; set; }
        public TimeSpan HRMOVIMENT
        {
            get
            {
                return _hrmoviment;
            }
            set
            {
                _hrmoviment = value;
                HoraMov = _hrmoviment.ToString(@"hh\:mm");
            }
        }
        public string HoraMov { get; set; }
        public decimal VLPRODMOV { get; set; }
        public int CDPRODUTO { get; set; }
        public string DSPRODUTO { get; set; }



    }
}