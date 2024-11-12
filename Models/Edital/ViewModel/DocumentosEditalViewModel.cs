namespace SiteSesc.Models.Edital.ViewModel
{
    public class DocumentosEditalViewModel
    {
        public edt_documentoEdital curriculos { get; set; }
        public List<edt_documentoEdital> documentos { get; set; }

        public edt_statusCurriculoEdital statusCurriculoEdital{ get; set; }

        public edt_parecerEdital parecerEdital { get; set; }   
    }
}
