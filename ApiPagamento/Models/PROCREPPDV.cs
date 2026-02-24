using System.ComponentModel.DataAnnotations;

namespace PagamentoApi.Models
{
    public class PROCREPPDV
    {

        [Key]
        public int CDPRODUTO { get; set; }
        public decimal MAXCREDITO { get; set; }
    }
}