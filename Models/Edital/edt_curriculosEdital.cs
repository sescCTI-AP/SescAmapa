using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Edital
{
    public class edt_curriculosEdital
    {
        public int Id { get; set; }

        public string Cpf { get; set; }

        public string NomeCandidato { get; set; }

        public string Telefone { get; set; }

        public string Email { get; set; }

        public string StatusCurriculo { get; set; }

        public string Justificativa { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [ForeignKey("edt_cargoEdital")]
        public int? IdCargoEdital { get; set; }

        [Display(Name = "CARGO")]
        [ForeignKey("IdCargoEdital")]
        public virtual edt_cargoEdital edt_cargoEdital { get; set; }
    }
}
