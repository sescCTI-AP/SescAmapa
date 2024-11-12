using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento.Relatorios
{
    public class GraficoInscricoesEvasoes
    {
        //public int ordenacao { get; set; }
        public int ano { get; set; }
        public int mes { get; set; }
        public string mesAno { get; set; }
        public int inscricoes { get; set; }
        public int evasoes { get; set; }
    }
}
