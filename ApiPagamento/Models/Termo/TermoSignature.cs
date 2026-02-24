using System;

namespace PagamentoApi.Models.Termo
{
    public class TermoSignature
    {

        public int Id { get; set; }
        public string Cdelement { get; set; }
        public string Cpf { get; set; }
        public string Termo64 { get; set; }
        public string NomeCliente { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
