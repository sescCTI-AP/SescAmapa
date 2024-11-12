using SiteSesc.Models.ApiPagamento;
using SiteSesc.Models.Atividade;

namespace SiteSesc.Models.ViewModel
{
    public class FinalizarInscricao
    {
        public ClienteCentral Cliente { get; set; }
        public AtividadeApi AtividadeApi { get; set; }
        public AtividadeOnLine AtividadeSite { get; set; }
        public string TemplateTermo { get; set; }
        public List<Horario> Horarios { get; set; }
        public FormasPgto FormasPgto { get; set; }
        public decimal Valor { get; set; }
    }
}
