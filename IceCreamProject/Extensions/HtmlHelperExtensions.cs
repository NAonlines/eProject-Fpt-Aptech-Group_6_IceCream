using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IceCreamProject.Extensions
{
    //Copy 

    public static class HtmlHelperExtensions
    {
        public static IHtmlContent DropdownOption(this IHtmlHelper htmlHelper, string value, string text, string selectedValue)
        {
            var isSelected = value == selectedValue ? "selected=\"selected\"" : string.Empty;
            return new HtmlString($"<option value=\"{value}\" {isSelected}>{text}</option>");
        }
    }

}
