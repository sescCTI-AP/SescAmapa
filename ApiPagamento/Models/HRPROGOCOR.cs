using System;
namespace PagamentoApi.Models
{
    public class HRPROGOCOR
    {
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public int SQHORARIO { get; set; }
        public int DDSEMANA { get; set; }
        public int DDMES { get; set; }
        public TimeSpan HRINICIO { get; set; }
        public TimeSpan HRFIM { get; set; }
        public DateTime DTINICIO { get; set; }
        public DateTime DTFIM { get; set; }
        public int STDISPONIV { get; set; }
    }
}