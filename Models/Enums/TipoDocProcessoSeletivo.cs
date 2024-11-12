using System.ComponentModel;
using System.Reflection;

namespace SiteSesc.Models.Enums
{
    public enum TipoDocProcessoSeletivo
    {
        [Description("Curriculo")]
        Curriculo = 1,

        [Description("Diploma")]
        Diploma = 2,

        [Description("Experiencia")]
        Experiencia = 3,
    }



}