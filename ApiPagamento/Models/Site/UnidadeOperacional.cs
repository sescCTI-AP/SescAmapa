using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace PagamentoApi.Models.Site
{
    public class UnidadeOperacional
    {
        public int Id { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "UOP")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public int? cduop { get; set; }

        [Display(Name = "GOOGLE MAPS")]
        public string Mapa { get; set; }

        [Display(Name = "INFORMAÇÃO")]
        public string Informacao { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [ForeignKey("Endereco")]
        public int IdEndereco { get; set; }

        public virtual ICollection<AtividadeOnLine> AtividadeOnLine { get; set; }

        [NotMapped]
        [Display(Name = "IMAGEM")]
        public IFormFile Imagem { get; set; }

    }
}