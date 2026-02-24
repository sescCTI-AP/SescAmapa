using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PagamentoApi.V2.Models.Pix
{
    [DataContract]
    public class PixInfoAdicionais
    {
        /// <summary>
        /// Nome do campo.
        /// </summary>
        /// <value>Nome do campo.</value>
        [Required]

        [MaxLength(50)]
        [DataMember(Name = "nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Dados do campo.
        /// </summary>
        /// <value>Dados do campo.</value>
        [Required]

        [MaxLength(200)]
        [DataMember(Name = "valor")]
        public string Valor { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
    }
}