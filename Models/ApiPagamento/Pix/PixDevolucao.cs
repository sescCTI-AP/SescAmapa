namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixDevolucao
    {
        public string id { get; set; }
        public string rtrId { get; set; }
        public decimal valor { get; set; }
        public PixHorario horario { get; set; }
        public string status { get; set; }
        public string motivo { get; set; }
    }
}
