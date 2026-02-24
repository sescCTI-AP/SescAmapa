using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    public class Desconto
    {
        /// <summary>
        /// Como o desconto será concedido, inteiro >= 0. Domínios:  0 - SEM DESCONTO; 1 - VLR FIXO ATE A DATA INFORMADA; 2 - PERCENTUAL ATE A DATA INFORMADA;  3 - DESCONTO POR DIA DE ANTECIPACAO.
        /// </summary>
        /// <value>Como o desconto será concedido, inteiro >= 0. Domínios:  0 - SEM DESCONTO; 1 - VLR FIXO ATE A DATA INFORMADA; 2 - PERCENTUAL ATE A DATA INFORMADA;  3 - DESCONTO POR DIA DE ANTECIPACAO.</value>
        [JsonProperty(PropertyName = "tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Se tipo > 0, Definir uma data de expiração do desconto, no formato \"dd.mm.aaaa\".
        /// </summary>
        /// <value>Se tipo > 0, Definir uma data de expiração do desconto, no formato \"dd.mm.aaaa\".</value>
        [JsonProperty(PropertyName = "dataExpiracao")]
        public string DataExpiracao { get; set; }

        /// <summary>
        /// Se tipo = 2, definir uma porcentagem de desconto >=  0.00 (formato decimal separado por \".\").
        /// </summary>
        /// <value>Se tipo = 2, definir uma porcentagem de desconto >=  0.00 (formato decimal separado por \".\").</value>
        [JsonProperty(PropertyName = "porcentagem")]
        public double? Porcentagem { get; set; }

        /// <summary>
        /// Se tipo = 1, definir um valor de desconto >=  0.00 (formato decimal separado por \".\").
        /// </summary>
        /// <value>Se tipo = 1, definir um valor de desconto >=  0.00 (formato decimal separado por \".\").</value>
        [JsonProperty(PropertyName = "valor")]
        public double? Valor { get; set; }
    }
}