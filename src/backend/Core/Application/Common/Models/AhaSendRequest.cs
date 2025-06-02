namespace EvrenDev.Infrastructure.Services.Model;
public class Content
{
    public string? Subject { get; set; }
    public string? TextBody { get; set; }
    public ContactDetails? ReplyTo { get; set; }
}

public class ContactDetails
{
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class AhasendRequest
{
    public ContactDetails? From { get; set; }
    public List<ContactDetails>? Recipients { get; set; }
    public Content? Content { get; set; }
}
