using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Admin
{
    public class adm_moduloSistema
    {
        public int Id { get; set; }

        [Display(Name = "NOME")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Nome { get; set; }
    }
}
