namespace SiteSesc.Models
{
    public class agd_tipoAgendamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool IsAtivo { get; set; }

        public virtual ICollection<agd_agendaAtividade> agd_agendaAtividade { get; set; }

    }
}
