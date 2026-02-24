namespace PagamentoApi.Models.BB
{
    public class Error
    {
        public int codigo { get; set; }
        public int versao { get; set; }
        public string mensagem { get; set; }
        public string ocorrencia { get; set; }
    }
}