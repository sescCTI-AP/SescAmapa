using System;

namespace PagamentoApi.V2.Responses
{
    public class PixResponse
    {
        public string Location { get; set; }
        public string Chave { get; set; }
        public string Txid { get; set; }
        public string PixCopiaECola { get; set; }
        public DateTime Criacao { get; set; }
        public string DataCriacao { get; set; }
        public string DataExpiracao { get; set; }
        public int Expiracao { get; set; }
        public decimal ValorOriginal { get; set; }
        public string Situacao { get; set; }
        public string SolicitacaoPagador { get; set; }
        public DevedorResponse Devedor { get; set; }
        public PagadorResponse Pagador { get; set; } = null ;


    }
    public class DevedorResponse
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }

    public class PagadorResponse
    {
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}
