using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class Beneficiario
    {
        /// <summary>
        /// Agência do beneficiário.
        /// </summary>
        /// <value>Agência do beneficiário.</value>
        [JsonProperty(PropertyName = "agencia")]
        public double? Agencia { get; set; }

        /// <summary>
        /// Número da conta corrente do beneficiário.
        /// </summary>
        /// <value>Número da conta corrente do beneficiário.</value>
        [JsonProperty(PropertyName = "contaCorrente")]
        public double? ContaCorrente { get; set; }

        /// <summary>
        /// Código do tipo de endereço do beneficiário.
        /// </summary>
        /// <value>Código do tipo de endereço do beneficiário.</value>
        [JsonProperty(PropertyName = "tipoEndereco")]
        public double? TipoEndereco { get; set; }

        /// <summary>
        /// Nome do logradouro do beneficiário.
        /// </summary>
        /// <value>Nome do logradouro do beneficiário.</value>
        [JsonProperty(PropertyName = "logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Bairro do Beneficiário.
        /// </summary>
        /// <value>Bairro do Beneficiário.</value>
        [JsonProperty(PropertyName = "bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Cidade do Beneficiário.
        /// </summary>
        /// <value>Cidade do Beneficiário.</value>
        [JsonProperty(PropertyName = "cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Identificador da cidade do beneficiário.
        /// </summary>
        /// <value>Identificador da cidade do beneficiário.</value>
        [JsonProperty(PropertyName = "codigoCidade")]
        public double? CodigoCidade { get; set; }

        /// <summary>
        /// Sigla do Estado do beneficiário.
        /// </summary>
        /// <value>Sigla do Estado do beneficiário.</value>
        [JsonProperty(PropertyName = "uf")]
        public string Uf { get; set; }

        /// <summary>
        /// Código Postal do Beneficiário.
        /// </summary>
        /// <value>Código Postal do Beneficiário.</value>
        [JsonProperty(PropertyName = "cep")]
        public double? Cep { get; set; }

        /// <summary>
        /// Indicador de prova de vida do beneficiário.
        /// </summary>
        /// <value>Indicador de prova de vida do beneficiário.</value>
        [JsonProperty(PropertyName = "indicadorComprovacao")]
        public string IndicadorComprovacao { get; set; }
    }
}