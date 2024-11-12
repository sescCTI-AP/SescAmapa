using SiteSesc.Models.Licitacao;

namespace SiteSesc.Models.Licitacao.ViewModel
{
    public class LicitacaoViewModel
    {
        public List<lct_licitacao> Aguardando { get; set; }
        public List<lct_licitacao> Abertas { get; set; }
        public List<lct_licitacao> EmAndamento { get; set; }
        public List<lct_licitacao> Finalizadas { get; set; }
    }
}
