using System;
using System.Collections.Generic;

namespace PagamentoApi.Models.Cielo
{
    public class ReturnPaymentTransacao : PaymentTransacao
    {
        public Guid PaymentId { get; set; }
        public decimal ServiceTaxAmount { get; set; }
        public string Interest { get; set; }
        public int Installments { get; set; }
        public bool Capture { get; set; }
        public bool Authenticate { get; set; }
        public bool IsCryptoCurrencyNegotiation { get; set; }
        public bool IsSplitted { get; set; }
        public bool TryAutomaticCancellation { get; set; }
        public bool IsQrCode { get; set; }
        public string ProofOfSale { get; set; }
        public string Tid { get; set; }
        public string AuthorizationCode { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
        public int Status { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public DateTime ReceivedDate { get; set; }
        public List<ReturnLinkAuthorization> Links { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public DateTime CapturedDate { get; set; }
        public int CapturedAmount { get; set; }
        public Card CreditCard { get; set; }
    }
}