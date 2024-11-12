using SiteSesc.Models.ProcessoSeletivo;
using SiteSesc.Models.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.MatriculaEscolar
{
    public class mat_escolar
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Unidade Operacional")]
        public int Unidade { get; set; } //Undidade Id - Trazer nome
        public string NomePublico { get; set; } //Nome de exibição
        public int Cidade { get; set; } //Cidade Turma
        public int Turma { get; set; } //Num Turma Central - Trazer nome
        public bool IsAtivo { get; set; } = true; //Ativo ou Inativo
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        [NotMapped]
        [ForeignKey("Unidade")]
        public virtual UnidadeOperacional IdUnidade { get; set; }

        [NotMapped]
        [ForeignKey("Cidade")]
        public virtual Cidade IdCidade { get; set; }

        [NotMapped]
        [ForeignKey("Turma")]
        public virtual Turma IdTurma { get; set; }
    }
}