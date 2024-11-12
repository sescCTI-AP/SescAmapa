using SiteSesc.Models.ApiPagamento;

namespace SiteSesc.Models.ViewModel
{
    public class CobrancaHistorico
    {
        public ClientelaViewModel Cliente { get; set; }
        public List<CobrancaAtualizada> Cobrancas { get; set; }
    }
}
