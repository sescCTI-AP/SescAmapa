namespace PagamentoApi.Models.Cielo
{
    public class Payment
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public bool Capture { get; set; }
        public int Installments { get; set; }
        public bool Recurrent { get; set; }
        public string SoftDescriptor { get; set; }
        public Card CreditCard { get; set; }

    }
}