using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Parentesco
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public virtual ICollection<SolicitacaoCadastroCliente> SolicitacaoCadastroCliente { get; set; }

    }
}
