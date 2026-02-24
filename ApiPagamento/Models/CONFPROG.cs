    using System.Collections.Generic;
    using System;
    namespace PagamentoApi.Models
    {

        public partial class CONFPROG
        {
            public int Id { get; protected set; }
            public int CDPROGRAMA { get; set; }
            public int CDCONFIG { get; set; }
            public string NMCONFIG { get; set; }
            public short CDUNIDTEMP { get; set; }
            public int? CDFORMATO { get; set; }
            public decimal? VLDURACAO { get; set; }
            public short VBPREINSCR { get; set; }
            public short? STPASSAGEM { get; set; }
            public short VBREJEITA { get; set; }
            public short VBTRANINSC { get; set; }
            public decimal? VLDURTRANC { get; set; }
            public short? STREINGREC { get; set; }
            public string TECONTEUDO { get; set; }
            public short VBCLIFORA { get; set; }
            public short VBABERTA { get; set; }
            public short? DDINICINSC { get; set; }
            public short? DDFIMINSC { get; set; }
            public short? DDRENOVACAO { get; set; }
            public string IDUSRRESP { get; set; }
            public DateTime DTATU { get; set; }
            public TimeSpan HRATU { get; set; }
            public string LGATU { get; set; }
            public short VBRODIZIO { get; set; }
            public int? CURSO { get; set; }

            public List<PROGOCORR> Turmas { get; set; }
        }
    }