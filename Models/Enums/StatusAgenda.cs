using System.ComponentModel;
using System.Reflection;

namespace SiteSesc.Models.Enums
{
    public enum StatusAgenda
    {
        [Description("Pendente")]
        Pendente = 0,

        [Description("Executado")]
        Executado = 1,

        [Description("Cancelado pelo cliente")]
        CanceladoPeloCliente = 2,

        [Description("Cancelado pelo atendente")]
        CanceladoPeloAtendente = 3
    }
    //public static string GetEnumDescription(Enum value)
    //{
    //    FieldInfo fi = value.GetType().GetField(value.ToString());

    //    DescriptionAttribute[] attributes =
    //        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

    //    if (attributes != null && attributes.Length > 0)
    //        return attributes[0].Description;
    //    else
    //        return value.ToString();
    //}

}
