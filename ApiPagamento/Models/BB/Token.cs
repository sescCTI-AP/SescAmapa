namespace PagamentoApi.Models.BB
{
    public class Token
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_in { get; set; }
    }
}