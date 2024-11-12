using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace SiteSesc.Models
{
    public class BotaoSideMenu
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string UrlRedirect { get; set; }
        public string Classe { get; set; }
        public bool IsDropdown { get; set; }
        public bool IsAdmin { get; set; }
        public int Sequencia { get; set; }
        public int IdModulo { get; set; }

        public virtual ICollection<BotaoPerfil> BotaoPerfil { get; set; }
        public virtual ICollection<BotaoDropDown> BotaoDropDown { get; set; }

    }
}
