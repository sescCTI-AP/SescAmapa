using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.SiteViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.ViewModel
{
    public class InscritoSituacaoCobranca
    {
        public CLIENTELA cliente { get; set; }
        public DateTime? dataInscricao { get; set; }
        public int? idade { get; set; }
        public DateTime? dtnascimento { get; set; }
        public string situação { get; set; }
        public string categoria { get; set; }
        public string classe { get; set; }
        public int cdfonteinf { get; set; }
        public List<Contato> contatos { get; set; }
    }
}
