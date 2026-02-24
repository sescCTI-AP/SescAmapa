using System.Collections.Generic;

namespace PagamentoApi.Models.Cielo
{
    public class ReturnTransacao
    {
        public int Status { get; set; }
        public int ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public string ProviderReturnCode { get; set; }
        public string ProviderReturnMessage { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string Tid { get; set; }
        public string ProofOfSale { get; set; }
        public string AuthorizationCode { get; set; }
        public List<ReturnLinkAuthorization> Links { get; set; }
        //dados preenchidos quando há um erro
        public int? Code { get; set; }
        public string Message { get; set; }
    }
}