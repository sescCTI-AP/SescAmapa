using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.ProcessoSeletivo.ViewModel
{
    public class CargoProcessoSeletivoViewModel
    {
        public int? Id { get; set; }

        public string Nome { get; set; }

        public int VagasCurriculo { get; set; }

        public int IdCidade { get; set; }

        public int IdProcessoSeletivo { get; set; }
        public bool HasDocExperiencia { get; set; }
        public bool HasDocFormacao { get; set; }

    }
}
