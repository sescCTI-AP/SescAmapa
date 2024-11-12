using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteSesc.Models.Relatorio
{
    public class RetornoMerchantOrder
    {
        public int reasonCode { get; set; }
        public string reasonMessage { get; set; }
        public List<PaymentRetorno> payments { get; set; }

    }
    public class PaymentRetorno
    {
        public string paymentId { get; set; }
        public DateTime receivedDate { get; set; }

    }
}
