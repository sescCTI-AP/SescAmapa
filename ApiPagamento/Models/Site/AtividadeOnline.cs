using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PagamentoApi.Models.Site
{
    public class AtividadeOnLine
    {
        public int Id { get; set; }
        public string NomeSite { get; set; }
        public int CdPrograma { get; set; }
        public int CdConfig { get; set; }
        public int SqOcorrenc { get; set; }
        public string Descricao { get; set; }
        public string UrlVideo { get; set; }
        public decimal? DescontoPontualidade { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int? IdadeMinima { get; set; }
        public int? IdadeMaxima { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFim { get; set; }
        public int Ano { get; set; }
        public bool Publicacao { get; set; }
        public DateTime? DataDesativacao { get; set; }
        public string PalavrasChaves { get; set; }
        public bool IsGratuito { get; set; }
        public bool IsDestaque { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAtivo { get; set; }
        public int? IdAtividadeAssociada { get; set; }
        public int? DescontoInscricao { get; set; }
        public DateTime? DataInicioPromocao { get; set; }
        public DateTime? DataFimPromocao { get; set; }

        [NotMapped]
        public decimal? ValorComerciario { get; set; }

        [NotMapped]
        public decimal? ValorGeral { get; set; }

        [ForeignKey("SubArea")]
        public int? IdSubArea { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("UnidadeOperacional")]
        public int IdUnidadeOperacional { get; set; }

        [Display(Name = "UNIDADE")]
        [ForeignKey("IdUnidadeOperacional")]
        public virtual UnidadeOperacional UnidadeOperacional { get; set; }
        public virtual ICollection<AvaliacaoAtividadeCliente> AvaliacaoAtividadeCliente { get; set; }
    }
}