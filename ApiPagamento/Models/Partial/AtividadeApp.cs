using System;

namespace PagamentoApi.Models.Partial
{
    public class AtividadeApp
    {
        public int CDPROGRAMA { get; set; }
        public int CDCONFIG { get; set; }
        public int SQOCORRENC { get; set; }
        public string Atividade { get; set; }
        public int CDFONTEINF { get; set; }

        public DateTime Inscricao { get; set; }
    }
}