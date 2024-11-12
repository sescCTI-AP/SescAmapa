using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class BotaoDropDown
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string UrlRedirect { get; set; }
        public string Classe { get; set; }

        [ForeignKey("BotaoSideMenu")]
        public int IdBotaoSideMenu { get; set; }

        [ForeignKey("IdBotaoSideMenu")]
        public virtual BotaoSideMenu BotaoSideMenu { get; set; }
    }
}
