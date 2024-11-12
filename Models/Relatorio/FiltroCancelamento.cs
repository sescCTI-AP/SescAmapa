namespace SiteSesc.Models.Relatorio
{
    public class FiltroCancelamento
    {
        public int? motivo { get; set; }
        public string cdelement { get; set; }
        public DateTime? dataInicial { get; set; }
        public DateTime? dataFinal { get; set; }
    }
}
