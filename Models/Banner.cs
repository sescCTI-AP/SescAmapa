using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Banner
    {

        public Banner()
        {
            this.DataCadastro = DateTime.Now;
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        [Display(Name = "Link")]
        public string? Url { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Está ativo?")]
        public bool IsAtivo { get; set; }

        //[NotMapped]
        //[Display(Name = "IMAGEM")]
        //public IFormFile Imagem { get; set; }
    }
}
