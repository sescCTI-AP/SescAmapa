using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Edital
{
    public class edt_observadoresEdital
    {
        public int? Id { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("edt_cargoEdital")]
        public int IdCargoEdital { get; set; }


        [Display(Name = "Usuario")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "edt_cargoEdital")]
        [ForeignKey("IdCargoEdital")]
        public virtual edt_cargoEdital edt_cargoEdital{ get; set; }
    }
}
