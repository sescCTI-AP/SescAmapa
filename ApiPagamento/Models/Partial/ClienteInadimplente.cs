using System;

namespace PagamentoApi.Models.Partial
{
    public class ClienteInadimplente
    {
        public string cpf { get; set; }
        public string  nome { get; set; }
        public int cduop { get; set; }
        public int? cduopcliente { get; set; }
        public int? sqmatric { get; set; }
        public DateTime dataInicial { get; set; }
        public DateTime dataFinal { get; set; }

    }
}
