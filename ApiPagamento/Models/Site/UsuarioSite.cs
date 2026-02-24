namespace PagamentoApi.Models.Site
{
    public class UsuarioSite
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public int IdPerfilUsuario { get; set; }
        public string Senha { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RefreshToken { get; set; }
        public string Token { get; set; }
        public bool DoisFatoresHabilitado { get; set; }
        public bool IsAtivo { get; set; }
    }
}