using System.Text;
using System.Text.RegularExpressions;

namespace EvrenDev.Application.Common.Functions
{
    public class Tools
    {
        public static Phone? CreatePhone(string? number, string? projectCode, string? banner)
        {
            return string.IsNullOrWhiteSpace(number) ? null : new Phone(number, projectCode, banner);
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

        private static string? GenerateWhatsappLink(string? input, string? projectCode, string? banner)
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

            if (!string.IsNullOrWhiteSpace(projectCode))
            {
                string fixedMessage =
                    "Hallo und Salam Aleykum,\n\n" +
                    "Vielen Dank für Ihre großzügige Unterstützung. %F0%9F%92%9A%F0%9F%A4%B2%F0%9F%8F%BC\n\n" +
                    "Dein Brunnencode lautet:\n\n" +
                    $"{projectCode}\n\n" +
                    $"{banner}\n\n" +
                    "Bitte genau durchlesen:\n\n" +
                    "Die Bauzeit beträgt ca. 8–16 Wochen. Ab der 8. Woche kannst du unter dem angegebenen Link nachsehen,\n" +
                    "ob der Brunnen fertiggestellt ist und zum Download bereitsteht.\n\n" +
                    "Alle „Vorher-/Nachher-Bilder“ und das Video werden zusammen nach der Fertigstellung hochgeladen.\n\n" +
                    "ACHTUNG:\n\n" +
                    "Bitte denkt daran, diese Daten mehrfach zu sichern, da sie nach etwa zwei Monaten von\n" +
                    "unserer Seite entfernt werden und danach schwer zugänglich sind. Sichert euch die Dateien rechtzeitig!\n\n" +
                    "https://brunnen.help-dunya.com/\n\n" +
                    "Möge Gott Ihre Spende annehmen und Ihre Taten segnen, sie vervielfachen und Ihnen im Dies- und Jenseits Gutes erweisen.\n\n" +
                    "https://help-dunya.com/\n\n" +
                    "Viele Grüße Help Dunya e.V.\n\n" +
                    "Über eine Bewertung würden wir uns sehr freuen! Vielen Dank im Voraus.\n\n" +
                    "https://g.page/HelpDunya/review?mpg.page\n\n" +
                    "https://m.facebook.com/pg/HelpDunya/reviews/\n\n" +
                    "https://help-dunya.com/help-dunya-e-v-rezension/";

                string encodedText = EncodeUtf8(fixedMessage);
                return $"{baseLink}?text={encodedText}";
            }

            return baseLink;
        }

        public class Phone(string? number, string? projectCode, string? banner)
        {
            public string? Whatsapp { get; set; } = GenerateWhatsappLink(input: number, projectCode: projectCode, banner: banner);
            public string? FormattedNumber { get; set; } = FormatPhoneNumber(input: number);
        }

        private static string EncodeUtf8(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                if ((b >= 0x30 && b <= 0x39) ||  // 0-9
                    (b >= 0x41 && b <= 0x5A) ||  // A-Z
                    (b >= 0x61 && b <= 0x7A) ||  // a-z
                    b == 0x2D || b == 0x2E || b == 0x5F || b == 0x7E) // - . _ ~
                {
                    builder.Append((char)b);
                }
                else
                {
                    builder.Append('%' + b.ToString("X2"));
                }
            }
            return builder.ToString();
        }

    }
}
