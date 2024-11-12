namespace SiteSesc.Models.Edital.ViewAction
{

    public class RequisicaoDto
    {
        public edt_edital Processo { get; set; }
        public List<AreaDto> Areas { get; set; }
        public List<CidadeDto> Cidades { get; set; }
        public List<Cargo> Cargos { get; set; }
    }

    public class AreaDto
    {
        public int Id { get; set; }
    }

    public class CidadeDto
    {
        public int Id { get; set; }
    }

    public class Cargo
    {
        public string Nome { get; set; }
        public int VagasCurriculo { get; set; }
        public int IdCidade { get; set; }
        public string Valor { get; set; }
        public string Projeto { get; set; }

        public bool HasDocExperiencia { get; set; }
        public bool HasDocFormacao { get; set; }
    }
}
