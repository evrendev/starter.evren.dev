using System.Text.RegularExpressions;
using System.Web;

namespace EvrenDev.Application.Common.Functions
{
    public class Tools
    {
        public static Phone? CreatePhone(string? number, string? message = null)
        {
            return string.IsNullOrWhiteSpace(number) ? null : new Phone(number, message);
        }

        private static string? FormatPhoneNumber(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string cleaned = Regex.Replace(input, @"[^\d+]", "");

            if (cleaned.StartsWith("00"))
                cleaned = "+" + cleaned.Substring(2);
            else if (cleaned.StartsWith("49"))
                cleaned = "+49" + cleaned.Substring(2);
            else if (cleaned.StartsWith("0"))
                cleaned = "+49" + cleaned.Substring(1);
            else if (cleaned.StartsWith("1"))
                cleaned = "+49" + cleaned;
            else if (!cleaned.StartsWith("+"))
                cleaned = "+49" + cleaned;

            var match = Regex.Match(cleaned, @"^\+49(\d{2,4})(\d+)$");
            if (match.Success)
                return $"+49 ({match.Groups[1].Value}) {match.Groups[2].Value}";

            return input;
        }

        private static string? GenerateWhatsappLink(string? input, string? text)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string cleaned = Regex.Replace(input, @"[^\d+]", "");

            if (cleaned.StartsWith("00"))
                cleaned = cleaned.Substring(2);
            else if (cleaned.StartsWith("+"))
                cleaned = cleaned.Substring(1);
            else if (cleaned.StartsWith("0"))
                cleaned = "49" + cleaned.Substring(1);
            else if (cleaned.StartsWith("1"))
                cleaned = "49" + cleaned;
            else if (cleaned.StartsWith("49"))
                cleaned = "49" + cleaned.Substring(2);
            else
                cleaned = "49" + cleaned;

            string baseLink = $"https://wa.me/{cleaned}";

            if (!string.IsNullOrWhiteSpace(text))
            {
                string encodedText = HttpUtility.UrlEncode(text);
                return $"{baseLink}?text={encodedText}";
            }

            return baseLink;
        }

        public class Phone(string? number, string? message = null)
        {
            public string? Whatsapp { get; set; } = GenerateWhatsappLink(input: number, text: message);
            public string? FormattedNumber { get; set; } = FormatPhoneNumber(input: number);
        }
    }
}
