using System.ComponentModel;

namespace SiteSesc.Models.Enums
{
    public enum PerfilUsuarioEnum
    {
        [Description("Administrador")]
        Administrador = 1,
        [Description("Cliente")]
        Cliente = 2,
        [Description("Coordernador")]
        Coordernador = 3,
        [Description("Funcionario")]
        Funcionario = 4,
        [Description("SysAdmin")]
        SysAdmin = 5
    }
}
