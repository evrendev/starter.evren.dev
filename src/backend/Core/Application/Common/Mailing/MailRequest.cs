using System.Text.Json.Serialization;

namespace EvrenDev.Application.Common.Mailing;

public class Content
{
    public string? Subject { get; set; }

    [JsonPropertyName("text_body")]
    public string? TextBody { get; set; }

    [JsonPropertyName("html_body")]
    public string? HtmlBody { get; set; }
}

public class Contact(string? email, string? name = null)
{
    public string? Email { get; set; } = email;
    public string? Name { get; set; } = name;
}

public class MailRequest(Content content, List<Contact>? recipients, Contact? from = null)
{
    public Contact? From { get; set; } = from;
    public List<Contact>? Recipients { get; set; } = recipients;
    public Content? Content { get; set; } = content;
}
