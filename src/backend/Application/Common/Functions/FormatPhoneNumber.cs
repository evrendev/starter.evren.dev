using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EvrenDev.Application.Common.Functions
{
    public class Tools
    {
        public static Phone? CreatePhone(string? number, string? project, string? banner)
        {
            return string.IsNullOrWhiteSpace(number) ? null : new Phone(number, project, banner);
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

        private static string? GenerateWhatsappLink(string? input, string? project, string? banner)
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

            string baseLink = $"https://api.whatsapp.com/send/?phone={phoneNumber}";

            if (!string.IsNullOrWhiteSpace(project))
            {
                string fixedMessage =
                    "Hallo und Salam Aleykum,\n\n" +
                    $"Vielen Dank für Ihre großzügige Unterstützung. 💚🤲🏼\n\n" +
                    "Dein Brunnencode lautet:\n\n" +
                    $"{project}:\n" +
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

                string encodedText = Uri.EscapeDataString(fixedMessage);
                return $"{baseLink}&text={encodedText}";
            }

            return baseLink;
        }

        public class Phone(string? number, string? project, string? banner)
        {
            public string? Whatsapp { get; set; } = GenerateWhatsappLink(input: number, project: project, banner: banner);
            public string? FormattedNumber { get; set; } = FormatPhoneNumber(input: number);
        }
    }
}
