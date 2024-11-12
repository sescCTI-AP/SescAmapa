using SiteSesc.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public partial class ClientelaViewModel
    {
        public int Id { get; protected set; }
        public int SQMATRIC { get; set; }
        public int CDUOP { get; set; }
        public short CDCATEGORI { get; set; }
        public string NMCLIENTE { get; set; }
        public string NMSOCIAL { get; set; }
        public string NUCNPJ { get; set; }
        public byte[] FOTO { get; set; }
        public DateTime DTNASCIMEN { get; set; }
        public DateTime DTVENCTO { get; set; }
        public string NUCPF { get; set; }
        public int? NUMCARTAO { get; set; }
        public string MATRICULA { get; set; }
        public string CATEGORIA { get; set; }
        public string CDBARRAS { get; set; }
        public int? TIPOCATEGORIA { get; set; }
        public string CDSEXO { get; set; }

        [NotMapped]
        public decimal? VLPARCELA { get; set; }

        [NotMapped]
        public string NomeAbrev
        {
            get
            {
                return Util.GetDuasPalavras(NMCLIENTE);
            }
        }

        [NotMapped]
        public string FotoCliente
        {
            get
            {
                if (FOTO != null)
                {
                    var imgBase64Dados = Convert.ToBase64String(FOTO);
                    var imagemCliente = $"data:image/png;base64,{imgBase64Dados}";
                    return imagemCliente;
                }
                return null;
            }
        }


    }
}
