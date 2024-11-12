using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.SiteViewModels
{
    public class DadosGraficoAtividade
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string NomeUnidade { get; set; }
        public int? ano { get; set; }
        public int inscricoes { get; set; }
        public int vagasDisponiveis { get; set; }
        public decimal? rating { get; set; }
        public string cidade { get; set; }
        public int idCduop { get; set; }
        public int cdprograma { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrenc { get; set; }
        public decimal? porcDisponviel { get; set; }
        public decimal? porcOcupada { get; set; }

        [NotMapped]
        [Display(Name = "CDELEMENT")]
        public string cdelement => $"{cdprograma.ToString().PadLeft(8, '0')}{cdconfig.ToString().PadLeft(8, '0')}{sqocorrenc.ToString().PadLeft(8, '0')}";
    }
}
