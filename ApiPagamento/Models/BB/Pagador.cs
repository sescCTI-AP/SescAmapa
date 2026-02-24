using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class Pagador
    {
        /// <summary>
        /// Código que identifica o tipo de inscrição do Pagador.  Domínios: 1 - CPF; 2 - CNPJ
        /// </summary>
        /// <value>Código que identifica o tipo de inscrição do Pagador.  Domínios: 1 - CPF; 2 - CNPJ</value>
        [JsonProperty(PropertyName = "tipoInscricao")]
        public int? TipoInscricao { get; set; }

        /// <summary>
        /// Número de inscrição do pagador.  Para o tipo = 1, informar numero do CPF.  Para o tipo = 2, informar numero do CNPJ.
        /// </summary>
        /// <value>Número de inscrição do pagador.  Para o tipo = 1, informar numero do CPF.  Para o tipo = 2, informar numero do CNPJ.</value>
        [JsonProperty(PropertyName = "numeroInscricao")]
        public long? NumeroInscricao { get; set; }

        /// <summary>
        /// Nome do pagador.
        /// </summary>
        /// <value>Nome do pagador.</value>
        [JsonProperty(PropertyName = "nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Endereço do pagador.
        /// </summary>
        /// <value>Endereço do pagador.</value>
        [JsonProperty(PropertyName = "endereco")]
        public string Endereco { get; set; }

        /// <summary>
        /// Código postal do pagador.
        /// </summary>
        /// <value>Código postal do pagador.</value>
        [JsonProperty(PropertyName = "cep")]
        public int? Cep { get; set; }

        /// <summary>
        /// Cidade do pagador.
        /// </summary>
        /// <value>Cidade do pagador.</value>
        [JsonProperty(PropertyName = "cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Bairro do pagador.
        /// </summary>
        /// <value>Bairro do pagador.</value>
        [JsonProperty(PropertyName = "bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Sigla do unidade federativa em que o pagador vive.
        /// </summary>
        /// <value>Sigla do unidade federativa em que o pagador vive.</value>
        [JsonProperty(PropertyName = "uf")]
        public string Uf { get; set; }

        /// <summary>
        /// Número de telefone do pagador.
        /// </summary>
        /// <value>Número de telefone do pagador.</value>
        [JsonProperty(PropertyName = "telefone")]
        public string Telefone { get; set; }
    }
}