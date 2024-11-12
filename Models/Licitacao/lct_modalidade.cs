using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Licitacao
{
    public class lct_modalidade
    {
        public int Id { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }
    }
}
