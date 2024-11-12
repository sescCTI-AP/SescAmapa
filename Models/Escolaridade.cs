namespace SiteSesc.Models
{
    public class Escolaridade
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<SolicitacaoCadastroCliente> SolicitacaoCadastroCliente { get; set; }

    }
}
