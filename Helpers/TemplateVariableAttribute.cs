using SiteSesc.Models.ApiPagamento;
using System.Reflection;

namespace SiteSesc.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TemplateVariableAttribute : Attribute
    {
        public string NomeVariavel { get; }

        public TemplateVariableAttribute(string nomeVariavel)
        {
            NomeVariavel = nomeVariavel;
        }
    }
}
