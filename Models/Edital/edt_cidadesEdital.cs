using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SiteSesc.Models.Edital;

namespace SiteSesc.Models.Edital
{
    public class edt_cidadesEdital
    {
        public int Id { get; set; }

        [ForeignKey("Cidade")]
        public int IdCidade { get; set; }

        [ForeignKey("edt_edital")]
        public int IdEdital { get; set; }



        [Display(Name = "CIDADE")]
        [ForeignKey("IdCidade")]
        public virtual Cidade Cidade { get; set; }

        [Display(Name = "EDITAL")]
        [ForeignKey("IdEdital")]
        public virtual edt_edital edt_edital { get; set; }
    }
}
