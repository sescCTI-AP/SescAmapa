namespace SiteSesc.Models.ApiPagamento.Cielo
{
    public class Payment
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public bool Capture { get; set; }
        public int Installments { get; set; } = 1;
        public bool Recurrent { get; set; }
        public string SoftDescriptor { get; set; }
        public Card CreditCard { get; set; }

        public Payment() { }

        public Payment(decimal amount, Card creditCard, int installments)
        {
            Amount = amount;
            CreditCard = creditCard;
            Installments = installments;
        }
    }
}
