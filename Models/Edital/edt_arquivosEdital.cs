using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Edital
{
    public class edt_arquivosEdital
    {
        public int Id { get; set; }

        [ForeignKey("Arquivo")]
        public int IdArquivo { get; set; }

        [ForeignKey("edt_edital")]
        public int IdEdital { get; set; }

        [Display(Name = "ARQUIVO")]
        [ForeignKey("IdArquivo")]
        public virtual Arquivo Arquivo { get; set; }

        [Display(Name = "EDITAL")]
        [ForeignKey("IdEdital")]
        public virtual edt_edital edt_Edital { get; set; }
    }
}
