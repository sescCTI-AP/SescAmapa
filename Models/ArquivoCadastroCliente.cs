using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class ArquivoCadastroCliente
    {
        public int Id { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [ForeignKey("SolicitacaoCadastroCliente")]
        public Guid IdSolicitacaoCadastroCliente { get; set; }

        [ForeignKey("IdSolicitacaoCadastroCliente")]
        public virtual SolicitacaoCadastroCliente SolicitacaoCadastroCliente { get; set; }

        public string Tipo { get; set; }
    }
}
