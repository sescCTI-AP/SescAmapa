using System.ComponentModel.DataAnnotations;

namespace SiteSesc.Domain
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    public class AreasAttributes : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            value = Convert.ToInt32(value);
            if (value is int intValue)
            {
                if (intValue <= 8 && intValue > 0)
                {
                    return true;
                }
                if (intValue == 0)
                {
                    value = null;
                    return true;
                }
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} deve corresponder a valores entre 1 e 8 campo ENUM.";
        }
    }

}
