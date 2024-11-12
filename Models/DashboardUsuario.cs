using Microsoft.Build.Evaluation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models
{
    public class DashboardUsuario
    {
        public int Id { get; set; }
        public DateTime DataRegistro { get; set; }
        public string Action { get; set; }

        public bool IsAtivo { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]

        public virtual Usuario Usuario { get; set; }
    }
}
