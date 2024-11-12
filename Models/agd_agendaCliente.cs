using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class agd_agendaCliente
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int StatusAgenda { get; set; } = 0;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public TimeSpan Horario { get; set; }
        public string Cpf { get; set; }

        public string NomeCliente { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Responsavel { get; set; }
        public string Telefone { get; set; }

        [ForeignKey("agd_agendaAtividade")]
        public int IdAgendaAtividade { get; set; }

        [ForeignKey("IdAgendaAtividade")]
        public virtual agd_agendaAtividade agd_agendaAtividade { get; set; }
    }
}
