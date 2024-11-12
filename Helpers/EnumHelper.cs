using System.ComponentModel;

namespace SiteSesc.Helpers
{
    public class EnumHelper
    {
        public static int GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (int)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Não encontrado", nameof(description));
        }
    }
}
