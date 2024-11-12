namespace SiteSesc.Models.ApiPagamento.Cielo
{
    public class Card
    {
        public Card() { }
        public Card(string cardNumber, string cardType, string holder, string expirationDate, string securityCode, string brand, bool saveCard = false)
        {
            CardNumber = cardNumber;
            CardType = cardType;
            Holder = holder;
            ExpirationDate = expirationDate;
            SecurityCode = securityCode;
            Brand = brand;
            SaveCard = saveCard;
        }
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public string Holder { get; set; }
        public string ExpirationDate { get; set; }
        public string SecurityCode { get; set; }
        public string Brand { get; set; }
        public string CardToken { get; set; }
        public bool SaveCard { get; set; }
    }
    public enum TypeCard
    {
        DebitCard,
        CreditCard
    }

    public enum BrandCard
    {
        Visa,
        Master,
        Elo
    }
}
