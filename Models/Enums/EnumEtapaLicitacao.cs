using System.ComponentModel;
using System.Reflection;

namespace SiteSesc.Models.Enums
{
    public enum EnumEtapaLicitacao
    {
        [Description("Aberto")]
        Aberto = 0,

        [Description("Andamento")]
        Andamento = 1,

        [Description("Finalizado")]
        Finalizado = 2
    }
}