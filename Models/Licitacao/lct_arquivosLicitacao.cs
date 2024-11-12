using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Licitacao
{
    public class lct_arquivosLicitacao
    {
        public int Id { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [ForeignKey("lct_licitacao")]
        public int IdLicitacao { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [Display(Name = "LICITAÇÃO")]
        [ForeignKey("IdLicitacao")]
        public virtual lct_licitacao lct_licitacao { get; set; }
    }
}
