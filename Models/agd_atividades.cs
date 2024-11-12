using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models
{
    public class agd_atividades
    {
        public int Id { get; set; }
        public string CdElement { get; set; }
        public bool IsAtivo { get; set; }

        [Required]
        [Display(Name = "Agenda")]
        public int IdAgendamento { get; set; }

        [Display(Name = "Agenda")]
        [ForeignKey("IdAgendamento")]
        public virtual agd_agendaAtividade agd_agendaAtividade { get; set; }

    }
}
