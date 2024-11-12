using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_observadoresProcessoSeletivo
    {
        public int? Id { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("psl_cargoProcessoSeletivo")]
        public int IdCargoProcessoSeletivo { get; set; }


        [Display(Name = "Usuario")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "psl_cargoProcessoSeletivo")]
        [ForeignKey("IdCargoProcessoSeletivo")]
        public virtual psl_cargoProcessoSeletivo psl_cargoProcessoSeletivo { get; set; }
    }
}
