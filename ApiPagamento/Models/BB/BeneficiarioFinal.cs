using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class BeneficiarioFinal
    {
        /// <summary>
        /// Código que identifica o tipo de inscrição do beneficiário final.  Domínios: 1 - CPF; 2 - CNPJ
        /// </summary>
        /// <value>Código que identifica o tipo de inscrição do beneficiário final.  Domínios: 1 - CPF; 2 - CNPJ</value>
        [JsonProperty(PropertyName = "tipoInscricao")]
        public int? TipoInscricao { get; set; }

        /// <summary>
        /// Número de registro do beneficiário final.  Para o tipo = 1, informar numero do CPF.  Para o tipo = 2, informar numero do CNPJ.
        /// </summary>
        /// <value>Número de registro do beneficiário final.  Para o tipo = 1, informar numero do CPF.  Para o tipo = 2, informar numero do CNPJ.</value>
        [JsonProperty(PropertyName = "numeroInscricao")]
        public long? NumeroInscricao { get; set; }

        /// <summary>
        /// Nome do beneficiário final
        /// </summary>
        /// <value>Nome do beneficiário final</value>
        [JsonProperty(PropertyName = "nome")]
        public string Nome { get; set; }
    }
}