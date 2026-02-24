using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagamentoApi.Models
{
    public class SolicitacaoReembolso
    {
        public int? Id { get; set; }
        public string? CpfCliente { get; set; }
        public string? CdElement { get; set; }
        public decimal? ValorReembolso { get; set; }
        public decimal? ValorSolicitado { get; set; }
        public string? Justificativa { get; set; }
        public int? OpcaoRecebimento { get; set; }
        public string? NomeFavorecido { get; set; }
        public string? NomeBanco { get; set; }
        public string? CpfFavorecido { get; set; }
        public string? Conta { get; set; }
        public int? TipoConta { get; set; }
        public int? Operacao { get; set; }
        public string? ChavePix { get; set; }
        public DateTime? DataCadastro { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdParentesco { get; set; }
        public int? IdStatusReembolso { get; set; }
        public string? Agencia { get; set; }
        public string? Telefone { get; set; }
        public DateTime? DataRevisao { get; set; }
        public int? IdUsuarioRevisao { get; set; }
        public string? MotivoIndeferido { get; set; }

    }
}
