using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models
{
    public class TelefoneEmpresa
    {
        public int Id { get; set; }

        [ForeignKey("Telefone")]
        public int IdTelefone { get; set; }

        [ForeignKey("Empresa")]
        public int IdEmpresa { get; set; }

        [Display(Name = "TELEFONE")]
        [ForeignKey("IdTelefone")]
        public virtual Telefone Telefone { get; set; }

        [Display(Name = "EMPRESA")]
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
    }
}
