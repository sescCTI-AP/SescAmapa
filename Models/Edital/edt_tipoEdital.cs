using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Edital
{
    public class edt_tipoEdital
    {
        public int Id { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }
    }
}
