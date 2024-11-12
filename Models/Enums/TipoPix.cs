using System.ComponentModel;

namespace SiteSesc.Models.Enums
{
    public enum TipoPix
    {
        [Description("Cobranca")]
        Cobranca = 1,

        [Description("Recarga")]
        Recarga = 2,

        [Description("Avulso")]
        Avulso = 3,

    }
}