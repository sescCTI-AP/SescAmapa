

namespace PagamentoApi.Models
{
    public class FORMASPGTO
    {
        public int CDFORMATO { get; set; }
        public int CDPROGRAMA { get; set; }
        public int DDVENCTO { get; set; }
        public string NMFORMATO  { get; set; }
        public int? NUPARCELAS { get; set; }
        // 0 - Mensal , 2 - A vista
        public int TPPGTO { get; set; }
        //Gerar Cobranças?
        public int VBCOBGERIN { get; set; }
        //Cobrar inscrição?
        public int VBINSCCOBR { get; set; }
        //Cobrar Proporcional?
        public int VBPROPORCI { get; set; }
        public int? VLPERIODIC { get; set; }
    }
}