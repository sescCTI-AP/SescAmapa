namespace SiteSesc.Models.EventoExterno
{
    public class EventoSummary
    {
        public int Id { get; set; }
        public string NomeEvento { get; set; }
        public int QuantidadeInscritos { get; set; }
        public int QuantidadeVagas { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool IsGratuito { get; set; }
        public bool IsAtivo { get; set; }
        public int IdArquvivo { get; set; }
        public string Unidade { get; set; }
        public string LocalEvento { get; set; }

        public decimal Valor { get; set; }
        public string Doacao { get; set; }

    }
}
