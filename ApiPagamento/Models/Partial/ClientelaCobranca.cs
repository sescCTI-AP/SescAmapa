using System;
using System.Collections.Generic;

namespace PagamentoApi.Models.Partial
{
    public class ClientelaCobranca
    {
        public string NmCliente { get; set; }
        public string MatFormat { get; set; }
        public string Contato { get; set; }
        public string Unidade { get; set; }
        public List<COBRANCA> Cobrancas { get; set; }
    }
}
