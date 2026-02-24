using System;
using System.Collections.Generic;
namespace PagamentoApi.Models
{
    public partial class PROGRAMAS
    {
        public int Id { get; protected set; }
        public int CDPROGRAMA { get; set; }
        public string NMPROGRAMA { get; set; }
        public string TECONTEUDO { get; set; }
        public short VBINSCRI { get; set; }
        public string DSDURACAO { get; set; }
        public string DSPERIODO { get; set; }
        public int? CDPROGSUP { get; set; }
        public DateTime DTATU { get; set; }
        public TimeSpan HRATU { get; set; }
        public string LGATU { get; set; }
        public short STATUS { get; set; }
        public int CDUOP { get; set; }
        public List<PROGRAMAS> SUBPROGRAMAS { get; set; }
        public List<PROGOCORR> TURMAS { get; set; }
    }
}