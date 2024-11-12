using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ApiPagamento.Relatorios
{
    public class InscricoesRelatorioEstatistica
    {
        public string CdElement { get; set; }
        public string NomePrograma { get; set; }
        public int ComerciariosInscritos { get; set; }
        public int DependentesComerciariosInscritos { get; set; }
        public int PublicoGeralInscritos { get; set; }
        public int ComerciariosEvasoes { get; set; }
        public int DependentesComEvasoes { get; set; }
        public int PublicoGeralEvasoes { get; set; }
        public int PcgInscritoComerciario { get; set; }
        public int PcgInscritoDependenteComerciario { get; set; }
        public int PcgInscritoPublicoGeral { get; set; }
        public int PcgEvasoesComerciario { get; set; }
        public int PcgEvasoesDependenteComerciario { get; set; }
        public int PcgEvasoesPublicoGeral { get; set; }

    }
}
