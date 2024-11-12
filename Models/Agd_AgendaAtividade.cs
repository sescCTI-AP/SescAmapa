using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.Admin;

namespace SiteSesc.Models
{
    public class agd_agendaAtividade
    {
        public int Id { get; set; }
        //public string Cdelement { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public TimeSpan TempoAtendimento { get; set; }
        public TimeSpan IntervaloAtendimento { get; set; }
        public int Ano { get; set; }
        public bool IsAtivo { get; set; } = true;

        [ForeignKey("agd_tipoAgendamento")]
        public int IdTipoAgendamento { get; set; }

        [Display(Name = "TipoAgendamento")]
        [ForeignKey("IdTipoAgendamento")]
        public virtual agd_tipoAgendamento TipoAgendamento { get; set; }

        public virtual ICollection<agd_agendaCliente> agd_agendaCliente { get; set; }
        public virtual ICollection<agd_atividades> agd_atividades { get; set; }


    }
}
