using System.Collections.Generic;

namespace PagamentoApi.Models.BB
{
    public class BoletoResponse
    {
        public string indicadorContinuidade { get; set; }
        public int quantidadeRegistros { get; set; }
        public int proximoIndice { get; set; }
        public List<Boleto> boletos { get; set; }
    }
}