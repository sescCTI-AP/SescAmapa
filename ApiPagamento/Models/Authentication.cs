using System.Security.AccessControl;

namespace PagamentoApi.Models
{
    public class Authentication
    {
        public Authentication(string App, string Token)
        {
            this.App = App;
            this.Token = Token;
        }
        public Authentication(string App, string AppSecret, string Email, string Senha)
        {
            this.App = App;
            this.AppSecret = AppSecret;
            this.Email = Email;
            this.Senha = Senha;
        }
        public Authentication() { }

        public int Id { get; set; }
        public string App { get; set; }
        public string AppSecret { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string GrantType { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }
        public string SecurityStamp { get; set; }
        public bool DoisFatoresHabilitado { get; set; }

    }
}