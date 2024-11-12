namespace SiteSesc.Models
{
    public class agd_agendamento
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string NomeCliente { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAgendamento { get; set; }
        public TimeSpan HorarioAgendamento { get; set; }
        public string Cdelement { get; set; }
    }
}
