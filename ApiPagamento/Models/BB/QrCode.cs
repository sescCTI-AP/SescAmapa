using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class QrCode
    {
        /// <summary>
        /// URL do payload do QR Code Pix 
        /// </summary>
        /// <value>URL do payload do QR Code Pix </value>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Codigo que identifica a transação Pix - transactionID
        /// </summary>
        /// <value>Codigo que identifica a transação Pix - transactionID</value>
        [JsonProperty(PropertyName = "txId")]
        public string TxId { get; set; }

        /// <summary>
        /// BR Code no padrão EMV. Sequência de caracteres correspondente ao payload do QR Code Pix. 
        /// </summary>
        /// <value>BR Code no padrão EMV. Sequência de caracteres correspondente ao payload do QR Code Pix. </value>
        [JsonProperty(PropertyName = "emv")]
        public string Emv { get; set; }
    }
}