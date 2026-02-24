using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagamentoApi.Models
{
    public class TermoReembolsoAssinado
    {
        public int? Id { get; set; }
        public string? Cdelement { get; set; }
        public string? Cpf { get; set; }
        public string? Termo64 { get; set; }
        public DateTime? DataCadastro { get; set; }
        public string? NomeCliente { get; set; }
        public int? TipoSignature { get; set; }
    }
}
