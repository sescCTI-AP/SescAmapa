using Newtonsoft.Json;

namespace PagamentoApi.Models.BB
{
    /// <summary>
    /// Código para identificação do tipo de multa para o Título de Cobrança, inteiro >= 0, 
    /// sendo: 0 - Sem multa, 1 - Valor da Multa, 2 - Percentual da Multa.  
    /// Se informado ‘0’ (zero) os campos “DATA DE MULTA”, “PERCENTUAL DE MULTA” e “VALOR DA MULTA” 
    /// não devem ser informados ou ser informados iguais a ‘0’ (zero).  O valor de juros e multa incidem 
    /// sobre o valor atual do boleto (valor do boleto - valor de abatimento).
    /// </summary>
    public class Multa
    {
        /// <summary>
        /// Como a multa será concedida, inteiro >= 0.  Domínios:  0 - DISPENSAR;  1 - VALOR DIA ATRASO;  2 - TAXA MENSAL;  3 - ISENTO.
        /// </summary>
        /// <value>Como a multa será concedida, inteiro >= 0.  Domínios:  0 - DISPENSAR;  1 - VALOR DIA ATRASO;  2 - TAXA MENSAL;  3 - ISENTO.</value>
        [JsonProperty(PropertyName = "tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Se tipo > 0, Definir uma data de multa, no formato \"dd.mm.aaaa\" 
        /// </summary>
        /// <value>Se tipo > 0, Definir uma data de multa, no formato \"dd.mm.aaaa\" </value>
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        /// <summary>
        /// Se tipo = 2,  definir porcentagem >=  0.00 (formato decimal separado por \".\").
        /// </summary>
        /// <value>Se tipo = 2,  definir porcentagem >=  0.00 (formato decimal separado por \".\").</value>
        [JsonProperty(PropertyName = "porcentagem")]
        public double? Porcentagem { get; set; }

        /// <summary>
        /// Se tipo = 1, definir valor do juros >=  0.00 (formato decimal separado por \".\").
        /// </summary>
        /// <value>Se tipo = 1, definir valor do juros >=  0.00 (formato decimal separado por \".\").</value>
        [JsonProperty(PropertyName = "valor")]
        public double? Valor { get; set; }
    }
}