using System.Text;
using System.Text.Json;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Services.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Services;

public class SendmailService : ISendmailService
{
    private readonly ILogger<SendmailService> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IStringLocalizer<SendmailService> _localizer;
    private readonly IConfiguration _configuration;

    public SendmailService(ILogger<SendmailService> logger,
        IHttpClientFactory clientFactory,
        IStringLocalizer<SendmailService> localizer,
        IConfiguration configuration)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _localizer = localizer;
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(AhasendRequest? request)
    {
        var ahaSendApiKey = _configuration.GetValue<string>("Ahasend:Apikey");
        var ahaSendApiUrl = _configuration.GetValue<string>("Ahasend:ApiUrl");
        var from = _configuration.GetSection("Ahasend:From").Get<ContactDetails>();
        var replyTo = _configuration.GetSection("Ahasend:ReplyTo").Get<ContactDetails>();

        var emailContent = new Content
        {
            Subject = request?.Content?.Subject,
            TextBody = request?.Content?.TextBody,
            ReplyTo = replyTo
        };

        var ahasendRequest = new AhasendRequest
        {
            From = from,
            Recipients = request?.Recipients,
            Content = emailContent
        };

        var sendinSerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
        var jsonContent = System.Text.Json.JsonSerializer.Serialize(ahasendRequest, sendinSerializeOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var httpClient = _clientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("x-api-key", ahaSendApiKey);

        var response = await httpClient.PostAsync(ahaSendApiUrl, content);

        return response.IsSuccessStatusCode;
    }
}
