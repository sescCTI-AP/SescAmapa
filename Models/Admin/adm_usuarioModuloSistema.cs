using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Admin
{
    public class adm_usuarioModuloSistema
    {
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("ModuloSistema")]
        public int IdModuloSistema { get; set; }

        [Display(Name = "USUÁRIO")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "MÓDULO DO SISTEMA")]
        [ForeignKey("IdModuloSistema")]
        public virtual adm_moduloSistema adm_moduloSistema { get; set; }

        public virtual ICollection<adm_acoesUsuarioModuloSistema> adm_acoesUsuarioModuloSistema { get; set; }
    }
}
