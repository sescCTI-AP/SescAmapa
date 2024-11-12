using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Admin
{
    public class adm_acoesUsuarioModuloSistema
    {
        public int Id { get; set; }

        [ForeignKey("UsuarioModuloSistema")]
        public int IdUsuarioModuloSistema { get; set; }

        [ForeignKey("AcaoSistema")]
        public int IdAcaoSistema { get; set; }

        [Display(Name = "USUÁRIO MÓDULO SISTEMA")]
        [ForeignKey("IdUsuarioModuloSistema")]
        public virtual adm_usuarioModuloSistema adm_usuarioModuloSistema { get; set; }

        [Display(Name = "AÇÃO DO SISTEMA")]
        [ForeignKey("IdAcaoSistema")]
        public virtual adm_acaoSistema adm_acaoSistema { get; set; }
    }
}
