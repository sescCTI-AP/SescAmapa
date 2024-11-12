using SiteSesc.Models.ApiPagamento.Cielo.ViewModel;
using SiteSesc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteSesc.Models.ApiPagamento.Cielo
{
    public class CartaoCredito
    {
        public int NumCartao { get; set; }
        public decimal Valor { get; set; }
        public ClienteCielo Customer { get; set; }
        public Payment Payment { get; set; }



        public CartaoCredito(DadosCobranca dados, PaymentViewModel payment) {

            var customer = new ClienteCielo { Name = payment.Name, Identity = payment.Identity};
            var cartao = new Card { CardNumber = payment.CardNumber, SecurityCode = payment.SecurityCode, ExpirationDate = payment.ExpirationDate, Brand = Util.IdentificarBandeira(payment.CardNumber)};
            var paymentInfo = new Payment { Amount = payment.Amount, CreditCard = cartao};

            NumCartao = Convert.ToInt32(dados.CDELEMENT);
            Valor = payment.Amount;
            Customer = customer;
            Payment = paymentInfo;
        }


    }
}