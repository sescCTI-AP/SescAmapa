namespace PagamentoApi.Models
{
    public class ModalidadeCategoria
    {
        public string CdElement { get; set; }
        public string CategoriaCliente { get; set; }
        public bool EComerciario { get; set; }
        public int QtdPorCategoria { get; set; }
        public string DsUsuario { get; set; }
        public int NuVagas { get; set; }
        public int NuVagasOcp { get; set; }
        //NUMINVOCUP
        public int NuMinOcup { get; set; }
        public int CdUopCad { get; set; }
        public string AaModa { get; set; }
    }
}
