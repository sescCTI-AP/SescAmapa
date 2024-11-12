using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class TelaInicialAdmin
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]

        public virtual Usuario Usuario { get; set; }
        public string Rota { get; set; }
    }
}
