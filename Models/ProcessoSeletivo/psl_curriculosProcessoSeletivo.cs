using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class edt_curriculosProcessoSeletivo
    {
        public int Id { get; set; }

        public string Cpf { get; set; }

        public string NomeCandidato { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [ForeignKey("psl_cargoProcessoSeletivo")]
        public int? IdCargoProcessoSeletivo { get; set; }

        [Display(Name = "CARGO")]
        [ForeignKey("IdCargoProcessoSeletivo")]
        public virtual psl_cargoProcessoSeletivo psl_cargoProcessoSeletivo { get; set; }
    }
}
