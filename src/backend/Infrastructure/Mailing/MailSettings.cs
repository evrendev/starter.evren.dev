using EvrenDev.Application.Common.Mailing;

namespace EvrenDev.Infrastructure.Mailing;

public class MailSettings
{
    public string? ApiUrl { get; set; }
    public string? ApiKey { get; set; }
    public Contact? From { get; set; }
    public Contact? ReplyTo { get; set; }
}
