using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace SiteSesc.CustomHtmlTags
{
    public class CheckboxListTagHelper : TagHelper
    {
        public IEnumerable<dynamic> Items { get; set; }
        public string Name { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var content = new StringBuilder();

            foreach (var item in Items)
            {
                content.AppendLine($@"
                <div class=""checkbox checkbox-inline checkbox-success"">
                    <input id=""item-{item.Id}"" name=""{Name}"" type=""checkbox"" value=""{item.Id}"">
                    <label for=""item-{item.Id}""><b>{item.Nome}</b></label>
                </div>
            ");
            }

            output.Content.SetHtmlContent(content.ToString());
        }
    }
}
