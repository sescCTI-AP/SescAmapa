using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models.Edital
{
    public class edt_areasEdital
    {
        public int Id { get; set; }

        [ForeignKey("Area")]
        public int IdArea { get; set; }

        [ForeignKey("edt_edital")]
        public int IdEdital { get; set; }

        [Display(Name = "ÁREA")]
        [ForeignKey("IdArea")]
        public virtual Area Area { get; set; }

        [Display(Name = "EDITAL")]
        [ForeignKey("IdEdital")]
        public virtual edt_edital edt_edital { get; set; }
    }
}
