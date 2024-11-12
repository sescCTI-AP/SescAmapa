using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Cardapio
{
    public class cdp_ComposicaoCardapio
    {
        public int Id { get; set; }

        public int? DiaDaSemana { get; set; }

        [ForeignKey("Cardapio")]
        public int IdCardapio { get; set; }


        [Display(Name = "CARDÁPIO")]
        [ForeignKey("IdCardapio")]
        public virtual cdp_Cardapio Cardapio { get; set; }


        [ForeignKey("ItemCardapio")]
        public int IdItemCardapio { get; set; }


        [Display(Name = "ITEM DO CARDÁPIO")]
        [ForeignKey("IdItemCardapio")]
        public virtual cdp_ItemCardapio ItemCardapio { get; set; }
    }
}