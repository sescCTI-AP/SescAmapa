using System.Text;
using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    /// <summary>
    /// Objeto de resposta da baixa de boletos.
    /// </summary>
    public class RespostaBaixaBoleto
    {
        /// <summary>
        /// Número do contrato de cobrança do boleto bancário.
        /// </summary>
        /// <value>Número do contrato de cobrança do boleto bancário.</value>
        [JsonProperty(PropertyName = "numeroContratoCobranca")]
        public string NumeroContratoCobranca { get; set; }

        /// <summary>
        /// Data do pedido de baixa do boleto bancário.
        /// </summary>
        /// <value>Data do pedido de baixa do boleto bancário.</value>
        [JsonProperty(PropertyName = "dataBaixa")]
        public string DataBaixa { get; set; }

        /// <summary>
        /// Horário do pedido de baixa do boleto bancário.HH:mm:ss
        /// </summary>
        /// <value>Horário do pedido de baixa do boleto bancário.HH:mm:ss</value>
        [JsonProperty(PropertyName = "horarioBaixa")]
        public string HorarioBaixa { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RespostaBaixaBoleto {\n");
            sb.Append("  NumeroContratoCobranca: ").Append(NumeroContratoCobranca).Append("\n");
            sb.Append("  DataBaixa: ").Append(DataBaixa).Append("\n");
            sb.Append("  HorarioBaixa: ").Append(HorarioBaixa).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Get the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}