namespace PagamentoApi.V2.Requests
{
    public class CriarPixRequest
    {
        public int CalendarioExpiracao { get; set; } = 600;
        public decimal ValorOriginal { get; set; }
        public string DevedorCpfCnpj { get; set; }
        public string DevedorNome { get; set; }
        public string solcnpjitacaoPagador { get; set; }

    }
}
