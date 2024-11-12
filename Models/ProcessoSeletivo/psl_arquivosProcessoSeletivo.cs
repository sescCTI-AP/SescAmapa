using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_arquivosProcessoSeletivo
    {
        public int Id { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [ForeignKey("psl_processoSeletivo")]
        public int IdProcessoSeletivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [Display(Name = "PROCESSO SELETIVO")]
        [ForeignKey("IdProcessoSeletivo")]
        public virtual psl_processoSeletivo psl_processoSeletivo { get; set; }
    }
}
