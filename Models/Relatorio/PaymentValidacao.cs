using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.Relatorio
{
    public class PaymentValidacao
    {
        public string merchantOrderId { get; set; }
        public Customer customer { get; set; }
        public PaymentValida payment { get; set; }
    }

    public class Customer
    {
        public string name { get; set; }
        public string identity { get; set; }
    }
    public class PaymentValida
    {
        public string paymentId { get; set; }
        public int installments { get; set; }
        public string tid { get; set; }
        public string authorizationCode { get; set; }
        public int status { get; set; }
        public DateTime receivedDate { get; set; }
        public DateTime captureDate { get; set; }
        public decimal amount { get; set; }

    }
}
