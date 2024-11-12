using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento
{
    public class InscricoesEvasoesViewModel
    {
        public int SeqMes { get; set; }
        public string Mes { get; set; }
        public int Inscricoes { get; set; }
        public int Evasoes { get; set; }
    }
}
