using System.ComponentModel.DataAnnotations.Schema;

namespace PagamentoApi.Models.Partial
{
    public class ClientelaSummary
    {
        public string NMCLIENTE { get; set; }
        public string NUCPF { get; set; }
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public int NUDV { get; set; }
        public string NMPAI { get; set; }
        public string NMMAE { get; set; }
        public short CDCATEGORI { get; set; }
        public string CATEGORIA { get; set; }
        public string NMSOCIAL { get; set; }
        public System.DateTime DTNASCIMEN { get; set; }
        public System.DateTime DTVENCTO { get; set; }

        [NotMapped]
        public string CREDENCIAL => $"{CDUOP.ToString().PadLeft(4, '0')}-{SQMATRIC.ToString().PadLeft(6, '0')}-{NUDV}";


    }
}
