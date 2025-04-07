using System.Text.RegularExpressions;

namespace Application.Common.Functions;

public class Tools
{
    public static string? FormatPhoneNumber(string input)
    {
        string cleaned = Regex.Replace(input, @"[^\d+]", "");

        if (cleaned.StartsWith("00"))
            cleaned = "+" + cleaned.Substring(2);
        else if (cleaned.StartsWith("49"))
            cleaned = "+49" + cleaned.Substring(2);
        else if (cleaned.StartsWith("0"))
            cleaned = "+49" + cleaned.Substring(1);
        else if (cleaned.StartsWith("1"))
            cleaned = "+49" + cleaned;
        else
            return null;

        var match = Regex.Match(cleaned, @"^\+49(\d{3})(\d+)$");
        if (match.Success)
            return $"+49 ({match.Groups[1].Value}) {match.Groups[2].Value}";

        return input;
    }
}
