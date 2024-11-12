using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class Evento
    {
        public Evento()
        {
            this.DataCadastro = DateTime.Now;
            this.Id = new Guid(Guid.NewGuid().ToString());
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Display(Name = "Nome")]   
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Display(Name = "Descricao")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Display(Name = "Data Inicial")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "Preenchimento obrigatório")]
        [Display(Name = "Data Final")]
        public DateTime DataFim { get; set; }

        [ForeignKey("Unidade")]

        public virtual UnidadeOperacional IdUnidade { get; set; }

        [Display(Name = "Unidade")]
        public int? Unidade { get; set; }

        [Display(Name = "Status")]
        public bool IsAtivo { get; set; } = true;

        [Display(Name = "Data de cadastro")]
        public DateTime DataCadastro { get; set; }

        [Display(Name = "Cidade")]
        public int? IdCidade { get; set; }

        [ForeignKey("IdCidade")]
        public virtual Cidade Cidade { get; set; }

        [ForeignKey("Area")]
        [Display(Name = "Área")]
        public int IdArea { get; set; }

        [Display(Name = "Área")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }


        [Display(Name = "SLUG")]
        public string slug { get; set; }
    }
}
