using System.Collections.Generic;

namespace PagamentoApi.Models.Cielo
{
    public class ReturnTokenizacao
    {
        public string CardToken { get; set; }
        //public ReturnLinkAuthorization Links { get; set; }
        //dados preenchidos quando há um erro
        public int? Code { get; set; }
        public string Message { get; set; }
    }
}
