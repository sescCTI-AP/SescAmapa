using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_tipoProcessoSeletivo
    {
        public int Id { get; set; }

        [Display(Name = "DESCRIÇÃO")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Descricao { get; set; }
    }
}
