using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class HstSolicitacao
    {
        public int Id { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public bool IsCliente { get; set; }

        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Solicitacao de Cadastro cliente")]
        public Guid IdSolicitacaoCadastroCliente { get; set; }

        [Display(Name = "Solicitacao de Cadastro cliente")]
        [ForeignKey("IdSolicitacaoCadastroCliente")]
        public virtual SolicitacaoCadastroCliente SolicitacaoCadastroCliente { get; set; }
    }
}
