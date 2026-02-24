using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
namespace PagamentoApi.Models.Pix
{
    [DataContract]
    public class PixValor
    {
        /// <summary>
        /// Gets or Sets Original
        /// </summary>
        [Required]

        [DataMember(Name = "original")]
        public decimal Original { get; set; }

    }
}