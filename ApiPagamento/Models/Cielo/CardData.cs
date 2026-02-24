namespace PagamentoApi.Models.Cielo
{
    public class CardData
    {
        public string Status { get; set; }
        public string Provider { get; set; }
        public string CardType { get; set; }
        public bool ForeignCard { get; set; }
        public bool CorporateCard { get; set; }
        public string Issuer { get; set; }
        public int IssuerCode { get; set; }
        //dados preenchidos quando há um erro
        public int? Code { get; set; }
        public string Message { get; set; }
    }
}