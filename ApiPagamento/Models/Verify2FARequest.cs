namespace PagamentoApi.Models
{
    public class Verify2FARequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string SecurityStamp { get; set; }
        public bool DoisFatoresHabilitado { get; set; }
        public string Nome { get; set; }
    }
}
