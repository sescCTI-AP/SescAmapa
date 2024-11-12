using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SiteSesc.Services
{
    public static class HtmlExtensions
    {
        public static IHtmlContent DisplayBoolAsText(this IHtmlHelper htmlHelper, bool value)
        {
            var span = new TagBuilder("span");
            span.InnerHtml.Append(value ? "Sim" : "Não");
            return span;
        }
    }
}
