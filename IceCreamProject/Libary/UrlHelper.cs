using System.Text.RegularExpressions;

using System.Text;

namespace IceCreamProject.Libary
{
    public class UrlHelper
    {
        public static string seoweb(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            string normalizedString = input.Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string noDiacritics = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            string slug = Regex.Replace(noDiacritics, @"[^a-zA-Z0-9\s-]", "").Trim();
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");

            return slug.ToLower();
        }
    }

}

