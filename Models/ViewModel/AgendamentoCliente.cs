using SiteSesc.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SiteSesc.Models.ViewModel
{
    public class AgendamentoCliente
    {
        public Guid Guid { get; set; }
        public string Descricao { get; set; }
        public int Dia { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public TimeSpan Horario { get; set; }
        public DateTime DataCadastro { get; set; }
        public int StatusAgenda { get; set; }

        [NotMapped]
        public DateTime DataAgendamento => new DateTime(Ano, Mes, Dia).Add(Horario);

        [NotMapped]
        public string StatusAgendaDescricao
        {
            get
            {
                StatusAgenda status = (StatusAgenda)StatusAgenda;
                FieldInfo fieldInfo = status.GetType().GetField(status.ToString());
                DescriptionAttribute attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                return attribute?.Description ?? status.ToString();
            }
        }
    }
}
