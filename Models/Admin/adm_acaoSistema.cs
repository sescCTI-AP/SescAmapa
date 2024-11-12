using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Admin
{
    public class adm_acaoSistema
    {
        public int Id { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        public virtual ICollection<adm_acoesUsuarioModuloSistema> adm_acoesUsuarioModuloSistema { get; set; }
    }
}
