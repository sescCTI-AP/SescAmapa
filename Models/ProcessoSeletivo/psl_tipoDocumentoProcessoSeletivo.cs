namespace SiteSesc.Models.ProcessoSeletivo
{
    public class psl_tipoDocumentoProcessoSeletivo
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<psl_documentoProcessoSeletivo> psl_documentoProcessoSeletivo { get; set; }
    }
}
