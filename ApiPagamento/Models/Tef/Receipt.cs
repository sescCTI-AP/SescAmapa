using System.Collections.Generic;

namespace PagamentoApi.Models.Tef
{
    public class Receipt
    {
        public int Cupom { get; set; }
        public string Data { get; set; }
        public List<object> Identificador { get; set; }
        public string CodigoPdv { get; set; }
        public List<Tef> Tef { get; set; }
    }
}