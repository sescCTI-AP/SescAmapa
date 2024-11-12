using SiteSesc.Models.ApiPagamento.Cielo.ViewModel;
using SiteSesc.Models.Relatorio;
using SiteSesc.Services;

namespace SiteSesc.Models.ApiPagamento.Cielo
{
    public class Transacao
    {
        public string MerchantOrderId { get; set; }
        public Payment Payment { get; set; }
        public ClienteCielo Customer { get; set; }
        public CobrancaValor CobrancaValor { get; set; }

        public Transacao() { }

        public Transacao(PaymentViewModel pay, CobrancaValor cobranca)
        {
            var customer = new ClienteCielo { Name = pay.Name, Identity = pay.Identity };
            var cartao = new Card { CardNumber = pay.CardNumber, SecurityCode = pay.SecurityCode, ExpirationDate = pay.ExpirationDate, Brand = Util.IdentificarBandeira(pay.CardNumber) };
            var pagamento = new Payment(pay.Amount, cartao, 1);

            Payment = pagamento;
            Customer = customer;
            CobrancaValor = cobranca;
        }
    }


}
