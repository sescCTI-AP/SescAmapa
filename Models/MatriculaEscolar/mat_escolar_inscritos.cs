using SiteSesc.Models.ProcessoSeletivo;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class mat_escolar_inscritos
    {
        public int Id { get; set; }
        public int idTurma { get; set; } //Num Pross. Matricula - Trazer nome
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [StringLength(11)]
        public string Cpf { get; set; }


    }
}