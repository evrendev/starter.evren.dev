using System.Text.RegularExpressions;

namespace EvrenDev.Application.Common.Functions
{
    public class Tools
    {
        public static Phone? CreatePhone(string? number)
        {
            return string.IsNullOrWhiteSpace(number) ? null : new Phone(number);
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

        private static string? GenerateWhatsappLink(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string phoneNumber = Regex.Replace(input, @"[^\d+]", "");

            if (phoneNumber.StartsWith("00"))
                phoneNumber = phoneNumber.Substring(2);
            else if (phoneNumber.StartsWith("+"))
                phoneNumber = phoneNumber.Substring(1);
            else if (phoneNumber.StartsWith("0"))
                phoneNumber = "49" + phoneNumber.Substring(1);
            else if (phoneNumber.StartsWith("1"))
                phoneNumber = "49" + phoneNumber;
            else if (phoneNumber.StartsWith("49"))
                phoneNumber = "49" + phoneNumber.Substring(2);
            else
                phoneNumber = "49" + phoneNumber;

            return $"https://api.whatsapp.com/send/?phone={phoneNumber}";
        }

        public class Phone(string? number)
        {
            public string? Whatsapp => GenerateWhatsappLink(input: number);
            public string? FormattedNumber => FormatPhoneNumber(input: number);
        }
    }
}
