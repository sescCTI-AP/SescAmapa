using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Org.BouncyCastle.Math.EC.ECCurve;
using SiteSesc.Models.ViewModel;

namespace SiteSesc.Models.Atividade
{
    public class AtividadeOnLine
    {

        public AtividadeOnLine()
        {
            DataCadastro = DateTime.Now;
            //Id = new Guid(Guid.NewGuid().ToString());
            IsAtivo = false;
        }
        public int Id { get; set; }
        public string NomeExibicao { get; set; }
        public int Cdprograma { get; set; }
        public int Cdconfig { get; set; }
        public int Sqocorrenc { get; set; }
        public string Descricao { get; set; }
        public int Ano { get; set; }
        public bool IsGratuito { get; set; }
        public bool IsOnline { get; set; }

        [ForeignKey("Arquivo")]
        public int? IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]

        public virtual Arquivo Arquivo { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public int IdUsuario { get; set; }

        [Required]
        [Display(Name = "SubArea")]
        public int IdSubArea { get; set; }

        [Required]
        [Display(Name = "Unidade Operacional")]
        public int IdUnidadeOperacional { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataDesativacao { get; set; }
        public bool IsAtivo { get; set; }
        public bool DescontoPontualidade { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFim { get; set; }
        public int? QuantidadeParcelas { get; set; }


        [Display(Name = "Usuário")]
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        [Display(Name = "Sub Área")]
        [ForeignKey("IdSubArea")]
        public virtual SubArea SubArea { get; set; }

        [Display(Name = "Usuário")]
        [ForeignKey("IdUnidadeOperacional")]
        public virtual UnidadeOperacional UnidadeOperacional { get; set; }

        [NotMapped]
        public string cdelement => $"{Cdprograma.ToString().PadLeft(8, '0')}{Cdconfig.ToString().PadLeft(8, '0')}{Sqocorrenc.ToString().PadLeft(8, '0')}";

        [NotMapped]
        public Turma turma => new Turma { CDPROGRAMA = Cdprograma, CDCONFIG = Cdconfig, SQOCORRENC = Sqocorrenc};

    }
}
