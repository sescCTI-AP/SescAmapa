using System.Collections.Generic;

namespace PagamentoApi.Models
{
    public class MODALIDADE
    {
        public int CDADMIN { get; set; }
        public int CDMAPA { get; set; }
        public int CDMODALIDA { get; set; }
        public int CDSUBMODAL { get; set; }
        public int CDREALIZAC { get; set; }
        public string DSMODALIDA { get; set; }
        public string DSSUBMODAL { get; set; }
        public string ANOPROG { get; set; }
        public List<MODALIDADE> SUBMODALIDADES { get; set; }
    }
}