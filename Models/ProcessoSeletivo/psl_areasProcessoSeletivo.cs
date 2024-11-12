using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_areasProcessoSeletivo
    {
        public int Id { get; set; }

        [ForeignKey("Area")]
        public int IdArea { get; set; }

        [ForeignKey("psl_processoSeletivo")]
        public int IdProcessoSeletivo { get; set; }

        [Display(Name = "ÁREA")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

        [Display(Name = "PROCESSO SELETIVO")]
        [ForeignKey("IdProcessoSeletivo")]
        public virtual psl_processoSeletivo psl_processoSeletivo { get; set; }
    }
}
