using Microsoft.AspNetCore.Mvc;
using SiteSesc.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SiteSesc.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;

namespace SiteSesc.Models
{
    public class Noticia
    {
        public Noticia()
        {
            DataCadastro = DateTime.Now;
            DataAtualizacao = DateTime.Now;
            Id = new Guid(Guid.NewGuid().ToString());
            IsAtivo = true;
            Views = 0;
        }

        [Key]
        [Display(Name = "Guid")]
        [Column("Guid")]
        public Guid Id { get; set; }


        [Required]
        [MinLength(15, ErrorMessage = "O texto informado é muito curto")]
        [Display(Name = "Titulo longo")]
        public string TituloLongo { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "O texto informado é muito curto")]
        [Display(Name = "Titulo curto")]
        public string TituloCurto { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        
        public virtual Usuario Usuario { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        public int? IdCidade { get; set; }


        [ForeignKey("IdCidade")]
        
        public virtual Cidade Cidade { get; set; }

        [ForeignKey("Unidade")]

        public virtual UnidadeOperacional IdUnidade { get; set; }


        [Display(Name = "Unidade")]
        public int? Unidade { get; set; }

        [Required]
        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; }

        [Required]
        [Display(Name = "Data de Lançamento")]
        public DateTime DataLancamento { get; set; }

        [Required]
        [Display(Name = "Data de atualizacao")]
        public DateTime DataAtualizacao { get; set; }

        [Display(Name = "Status")]
        public bool IsAtivo { get; set; }

        [Display(Name = "Views")]
        public int? Views { get; set; }

        [Required]
        [MinLength(150, ErrorMessage = "O texto informado é muito curto")]
        [Display(Name = "Corpo da notícia")]
        public string CorpoNoticia { get; set; }

        [Display(Name = "Crédito da imagem")]
        public string? CreditoImagem { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        
        public virtual Arquivo Arquivo { get; set; }

        [ForeignKey("Area")]
        [Display(Name = "Área")]
        public int IdArea { get; set; }

        [Display(Name = "Área")]
        [ForeignKey("IdArea")]
        [JsonIgnore]
        
        public virtual Area Area { get; set; }

        [Display(Name = "Slug")]
        public string Slug { get; set; }

       


        //[NotMapped]
        //[Display(Name = "IMAGEM")]
        //public IFormFile Imagem { get; set; }
    }

}