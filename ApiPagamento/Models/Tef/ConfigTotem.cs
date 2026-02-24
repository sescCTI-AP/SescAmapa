using System.Security.Permissions;

namespace PagamentoApi.Models.Tef
{
    public class ConfigTotem
    {
        public int ID { get; set; }
        public string CODTOTEM { get; set; }
        public string IPTOTEM { get; set; }
        public string IPIMPRESSORA { get; set; }
        public int PORTAIMPRESSORA { get; set; }
        public bool ATIVO { get; set; }
    }
}
