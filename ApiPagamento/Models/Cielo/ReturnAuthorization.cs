namespace PagamentoApi.Models.Cielo
{
    public class ReturnAuthorization
    {
        public string MerchantOrderId { get; set; }
        public Cliente Customer { get; set; }
        public ReturnPaymentTransacao Payment { get; set; }
        //dados preenchidos quando há um erro
        public int? Code { get; set; }
        public string Message { get; set; }
    }
}