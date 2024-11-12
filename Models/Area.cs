using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.ProcessoSeletivo;

namespace SiteSesc.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IsAtivo { get; set; }
        public string NameRoute { get; set; }
        public string? Descricao { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        public virtual ICollection<Noticia> Noticia { get; set; }
        public virtual ICollection<psl_processoSeletivo> psl_processoSeletivo { get; set; }
    }
}
