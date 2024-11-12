namespace SiteSesc.Models.ViewModel
{
    public class UsuarioList
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Perfil { get; set; }
        public bool IsAtivo { get; set; }
        public DateTime DataCadastro { get; set; }

        public string Status => IsAtivo ? "Ativo" : "Desativado";
        public string ClassStatus => IsAtivo ? "ativo" : "desativado";
    }
}
