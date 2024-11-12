using Org.BouncyCastle.Bcpg.OpenPgp;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ApiPagamento.Cielo.ViewModel
{
    public class PaymentViewModel
    {
        public string Name { get; set; }
        public string Identity { get; set; }
        public string CardNumber { get; set; }
        public string SecurityCode { get; set; }
        public string MonthExpiration { get; set; }
        public string YearExpiration { get; set; }
        public decimal Amount { get; set; }

        [NotMapped]
        public string ExpirationDate => $"{MonthExpiration}/{YearExpiration}";
    }
}
