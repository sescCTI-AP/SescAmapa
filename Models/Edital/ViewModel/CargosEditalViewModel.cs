using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Edital.ViewModel
{
    public class CargoEditalViewModel
    {
        public int? Id { get; set; }

        public string Nome { get; set; }

        public int VagasCurriculo { get; set; }

        public string Valor { get; set; }

        public string Projeto { get; set; }
        public int IdCidade { get; set; }

        public int IdEdital { get; set; }
        public bool HasDocExperiencia { get; set; }
        public bool HasDocFormacao { get; set; }

    }
}
