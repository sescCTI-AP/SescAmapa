using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.ProcessoSeletivo.ViewAction;

namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_cargoProcessoSeletivo
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int VagasCurriculo { get; set; }

        public bool? HasDocFormacao { get; set; }
        public bool? HasDocExperiencia { get; set; }

        [ForeignKey("Cidade")]
        public int IdCidade { get; set; }

        [Display(Name = "Cidade")]
        [ForeignKey("IdCidade")]
        public virtual Cidade Cidade { get; set; }


        [ForeignKey("psl_processoSeletivo")]
        public int IdProcessoSeletivo { get; set; }

        [Display(Name = "PROCESSO SELETIVO")]
        [ForeignKey("IdProcessoSeletivo")]
        public virtual psl_processoSeletivo psl_processoSeletivo { get; set; }
        public virtual ICollection<psl_observadoresProcessoSeletivo> psl_observadoresProcessoSeletivo { get; set; }
        public virtual ICollection<psl_documentoProcessoSeletivo> psl_documentoProcessoSeletivo { get; set; }

        public psl_cargoProcessoSeletivo ToCargo(Cargo cargo)
        {
            return new psl_cargoProcessoSeletivo
            {
                Nome = cargo.Nome,
                VagasCurriculo = cargo.VagasCurriculo,
                HasDocFormacao = cargo.HasDocFormacao,
                HasDocExperiencia = cargo.HasDocExperiencia
            };
        }

    }
}
