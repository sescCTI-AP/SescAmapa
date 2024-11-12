using SiteSesc.Models.Atividade;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class SubArea
    {
        public int Id { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "DATA DA ÚLTIMA ATUALIZAÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public DateTime DataAtualizacao { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [Display(Name = "IMAGEM")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [NotMapped]
        [Display(Name = "IMAGEM")]
        public IFormFile Imagem { get; set; }

        [ForeignKey("Area")]
        public int? IdArea { get; set; }

        [Display(Name = "ÁREA")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

        public virtual ICollection<AtividadeOnLine> AtividadeOnLine { get; set; }
    }
}
