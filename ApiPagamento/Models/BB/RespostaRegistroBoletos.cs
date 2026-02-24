using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    /// <summary>
    /// Representação dos campos de resposta de uma solicitação de criação de boletos bancários.
    /// </summary>
    public class RespostaRegistroBoletos
    {
        /// <summary>
        /// Identificador exclusivo do boleto.
        /// </summary>
        /// <value>Identificador exclusivo do boleto.</value>
        [JsonProperty(PropertyName = "numero")]
        public string Numero { get; set; }

        /// <summary>
        /// Número da carteira do convênio de cobrança
        /// </summary>
        /// <value>Número da carteira do convênio de cobrança</value>
        [JsonProperty(PropertyName = "numeroCarteira")]
        public double? NumeroCarteira { get; set; }

        /// <summary>
        /// Número da variação da carteira do convênio de cobrança
        /// </summary>
        /// <value>Número da variação da carteira do convênio de cobrança</value>
        [JsonProperty(PropertyName = "numeroVariacaoCarteira")]
        public double? NumeroVariacaoCarteira { get; set; }

        /// <summary>
        /// Identificação do cliente.
        /// </summary>
        /// <value>Identificação do cliente.</value>
        [JsonProperty(PropertyName = "codigoCliente")]
        public double? CodigoCliente { get; set; }

        /// <summary>
        /// Linha digitável do boleto.
        /// </summary>
        /// <value>Linha digitável do boleto.</value>
        [JsonProperty(PropertyName = "linhaDigitavel")]
        public string LinhaDigitavel { get; set; }

        /// <summary>
        /// Define o código de barras numérico do boleto.
        /// </summary>
        /// <value>Define o código de barras numérico do boleto.</value>
        [JsonProperty(PropertyName = "codigoBarraNumerico")]
        public string CodigoBarraNumerico { get; set; }

        /// <summary>
        /// Define o número do contrato de cobrança do boleto.
        /// </summary>
        /// <value>Define o número do contrato de cobrança do boleto.</value>
        [JsonProperty(PropertyName = "numeroContratoCobranca")]
        public double? NumeroContratoCobranca { get; set; }

        /// <summary>
        /// Gets or Sets Beneficiario
        /// </summary>
        [JsonProperty(PropertyName = "beneficiario")]
        public Beneficiario Beneficiario { get; set; }

        /// <summary>
        /// Gets or Sets QrCode
        /// </summary>
        [JsonProperty(PropertyName = "qrCode")]
        public QrCode QrCode { get; set; }
    }
}