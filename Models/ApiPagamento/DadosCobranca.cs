using Microsoft.Identity.Client;

namespace SiteSesc.Models.ApiPagamento
{
    public class DadosCobranca
    {
        public string CPF { get; set; }
        public int CDUOP { get; set; }
        public int SQMATRIC { get; set; }
        public string IDCLASSE { get; set; }
        public string CDELEMENT { get; set; }
        public short SQCOBRANCA { get; set; }
        public string? VALOR { get; set; } 
        public string? DESCRICAO { get; set; } 
    }
}
