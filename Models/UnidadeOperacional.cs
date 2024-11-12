using Humanizer;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace SiteSesc.Models
{


    public class UnidadeOperacional
    {
        public UnidadeOperacional()
        {
            IsAtivo = true;
        }
        public int Id { get; set; }
        public int Cduop { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }

        [Display(Name = "Status")]
        public bool IsAtivo { get; set; }
        public string? Descricao { get; set; }
        public string? NomeCurto { get; set; }
        public string? NameRoute { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]

        public virtual Arquivo Arquivo { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

    }
}
