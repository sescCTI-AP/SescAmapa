using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PagamentoApi.Models.Site
{
    public class AvaliacaoAtividadeCliente
    {
        public int Id { get; set; }

        [FromForm(Name = "cdprograma")]
        public int cdprograma { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrec { get; set; }
        public string comentario { get; set; }
        public int rating { get; set; }
        public DateTime? DataCadastro { get; set; }

        [ForeignKey("AtividadeOnLine")]
        public int? IdAtividadeOnLine { get; set; }

        [Display(Name = "ATIVIDADE ONLINE")]
        [ForeignKey("IdAtividadeOnLine")]
        public virtual AtividadeOnLine AtividadeOnLine { get; set; }

        [ForeignKey("Cliente")]
        public int IdCliente { get; set; }
    }
}