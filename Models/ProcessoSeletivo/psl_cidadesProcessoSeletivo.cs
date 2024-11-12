using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_cidadesProcessoSeletivo
    {
        public int Id { get; set; }

        [ForeignKey("Cidade")]
        public int IdCidade { get; set; }

        [ForeignKey("psl_processoSeletivo")]
        public int IdProcessoSeletivo { get; set; }



        [Display(Name = "CIDADE")]
        [ForeignKey("IdCidade")]
        public virtual Cidade Cidade { get; set; }

        [Display(Name = "PROCESSO SELETIVO")]
        [ForeignKey("IdProcessoSeletivo")]
        public virtual psl_processoSeletivo psl_processoSeletivo { get; set; }
    }
}
