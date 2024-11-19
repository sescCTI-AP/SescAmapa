using System.ComponentModel;

namespace SiteSesc.Models.Enums
{
    public enum PerfilUsuarioEnum
    {
        [Description("Administrador")]
        Administrador = 2,
        [Description("Cliente")]
        Cliente = 4,
        [Description("Funcionario")]
        Funcionario = 3,
        [Description("SysAdmin")]
        SysAdmin = 1
    }
}
