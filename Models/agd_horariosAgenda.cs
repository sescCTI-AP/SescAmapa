using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class agd_horariosAgenda
    {
        public int Id { get; set; }

        [Required]
        public int Mes { get; set; }

        [Required]
        public int Dia { get; set; }

        [Required]
        [Display(Name = "agd_agendaAtividade")]
        public int idAgendamento { get; set; }

        [ForeignKey("idAgendamento")]
        public virtual agd_agendaAtividade agd_agendaAtividade { get; set; }

        [Required]
        public TimeSpan Hora { get; set; }
    }
}
