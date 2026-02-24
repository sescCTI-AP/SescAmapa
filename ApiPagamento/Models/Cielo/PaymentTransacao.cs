namespace PagamentoApi.Models.Cielo
{
    public class PaymentTransacao
    {
        public string MerchantOrderId { get; set; }
        public Payment Payment { get; set; }
        public Cliente Customer { get; set; }
    }
}