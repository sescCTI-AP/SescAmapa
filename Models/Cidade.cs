using SiteSesc.Models.ProcessoSeletivo;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Cidade
    {
        public Cidade() { 
            IsAtivo = true;
        }
        public int Id { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        public string Nome { get; set; }

        [Display(Name = "Status")]
        public bool IsAtivo { get; set; }


        public virtual ICollection<Noticia> Noticia { get; set; }
        public virtual ICollection<psl_cidadesProcessoSeletivo> psl_cidadesProcessoSeletivo { get; set; }


    }
}
