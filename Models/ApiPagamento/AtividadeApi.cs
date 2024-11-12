using SiteSesc.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento
{
    public class AtividadeApi
    {
        public int id { get; set; }
        public int cdprograma { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrenc { get; set; }

        [TemplateVariable("NomeAtividade")]
        public string dsusuario { get; set; }
        public decimal? idademin { get; set; }
        public decimal? idademax { get; set; }
        public DateTime? dtiniocorr { get; set; }
        public DateTime? dtfimocorr { get; set; }
        public DateTime? dtlinscri { get; set; }
        public DateTime? dtuinscri { get; set; }
        public int nuvagas { get; set; }
        public int nuvagasocp { get; set; }
        public int cduopcad { get; set; }
        public string aamoda { get; set; }
        public List<FormasPgto> formaspgto { get; set; }
        public List<Horario>? horarios { get; set; }
        public int? cdmapa { get; set; }

        [NotMapped]
        public string cdelement => $"{cdprograma.ToString().PadLeft(8, '0')}{cdconfig.ToString().PadLeft(8, '0')}{sqocorrenc.ToString().PadLeft(8, '0')}";

    }

    public class FormasPgto
    {
        public int cdformato { get; set; }
        public int cdprograma { get; set; }
        public int ddvencto { get; set; }
        public string nmformato { get; set; }
        public int tppgto { get; set; }
    }

    public class Horario
    {
        //public HoraInicio hrinicio { get; set; }
        //public HoraFim hrfim { get; set; }        
        public string hrinicio { get; set; }
        public string hrfim { get; set; }
        public int cdprograma { get; set; }
        public int cdconfig { get; set; }
        public int sqocorrenc { get; set; }
        public string idclasse { get; set; }
        public string cdelement { get; set; }
        public int sqhorario { get; set; }
        public int ddsemana { get; set; }
        public int ddmes { get; set; }
        public DateTime? dtinicio { get; set; }
        public DateTime? dtfim { get; set; }
        public int stdisponiv { get; set; }
    }

    public class HoraInicio
    {
        public int hours { get; set; }
        public int minutes { get; set; }

        [NotMapped]
        public string horarioInicial => $"{hours.ToString().PadLeft(2, '0')}:{minutes.ToString().PadLeft(2, '0')}";

    }
    public class HoraFim
    {
        public int hours { get; set; }
        public int minutes { get; set; }
        public string horarioFinal => $"{hours.ToString().PadLeft(2, '0')}:{minutes.ToString().PadLeft(2, '0')}";

    }
}
