using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.Edital.ViewAction;

namespace SiteSesc.Models.Edital
{
    public class edt_cargoEdital
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int VagasCurriculo { get; set; }
        public string Valor { get; set; }
        public string Projeto { get; set; }
        public bool? HasDocFormacao { get; set; }
        public bool? HasDocExperiencia { get; set; }

        [ForeignKey("Cidade")]
        public int IdCidade { get; set; }

        [Display(Name = "Cidade")]
        [ForeignKey("IdCidade")]
        public virtual Cidade Cidade { get; set; }


        [ForeignKey("edt_edital")]
        public int IdEdital { get; set; }

        [Display(Name = "EDITAL")]
        [ForeignKey("IdEdital")]
        public virtual edt_edital edt_edital { get; set; }
       
        public edt_cargoEdital ToCargo(Cargo cargo)
        {
            return new edt_cargoEdital
            {
                Nome = cargo.Nome,
                Valor = cargo.Valor,
                Projeto = cargo.Projeto,
                VagasCurriculo = cargo.VagasCurriculo,
                HasDocFormacao = cargo.HasDocFormacao,
                HasDocExperiencia = cargo.HasDocExperiencia
            };
        }

    }
}
