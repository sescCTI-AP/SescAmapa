using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class PerfilUsuario
    {
        public int Id { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }

        public virtual ICollection<Usuario> Usuario { get; set; }
        public virtual ICollection<BotaoPerfil> BotaoPerfil { get; set; }
    }
}
