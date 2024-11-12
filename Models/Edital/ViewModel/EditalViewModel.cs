namespace SiteSesc.Models.ProcessoSeletivo.ViewModel
{
    public class EditalViewModel
    {
        public List<psl_processoSeletivo> Aguardando { get; set; }
        public List<psl_processoSeletivo> Abertas { get; set; }
        public List<psl_processoSeletivo> EmAndamento { get; set; }
        public List<psl_processoSeletivo> Finalizadas { get; set; }
    }
}
