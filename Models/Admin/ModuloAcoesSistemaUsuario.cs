using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSesc.Models.Admin
{
    [NotMapped]
    public class ModuloAcoesSistemaUsuario
    {
        public int IdModulo { get; set; }
        public int[] Acoes { get; set; }
    }
}
