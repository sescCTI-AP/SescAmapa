using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ApiPagamento.Pix
{
    public class PixInfoAdicionais
    {
        [Required, MaxLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public string nome { get; set; }

        [Required, MaxLength(200, ErrorMessage = "Máximo de 200 caracteres")]
        public string valor { get; set; }
    }
}
