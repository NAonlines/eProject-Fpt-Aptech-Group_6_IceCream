using System.Text.RegularExpressions;

namespace IceCreamProject.Extensions
{
    //Copy 

    public static class PackageHelper
    {
        public static DateTime ToDate(this string Duration)
        {
            if (string.IsNullOrWhiteSpace(Duration))
            {
                return DateTime.Now;
            }
            // Xử lý chuỗi Duration 
            var match = System.Text.RegularExpressions.Regex.Match(Duration, @"(\d+)\s*(day|month|year)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                int number = int.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value.ToLower();

                switch (unit)
                {
                    case "day":
                        return DateTime.Now.AddDays(number);
                    case "month":
                        return DateTime.Now.AddMonths(number);
                    case "year":
                        return DateTime.Now.AddYears(number);
                }
            }

            return DateTime.Now;

        }
    }
}
