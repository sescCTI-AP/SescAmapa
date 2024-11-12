using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Models
{
    public class BotaoPerfil
    {
        public int Id { get; set; }

        [ForeignKey("BotaoSideMenu")]
        public int IdBotaoSideMenu { get; set; }

        [ForeignKey("IdBotaoSideMenu")]
        public virtual BotaoSideMenu BotaoSideMenu { get; set; }

        [ForeignKey("PerfilUsuario")]
        public int IdPerfilUsuario { get; set; }

        [ForeignKey("IdPerfilUsuario")]
        public virtual PerfilUsuario PerfilUsuario { get; set; }
    }
}
