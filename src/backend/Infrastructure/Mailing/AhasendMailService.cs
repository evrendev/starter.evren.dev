using System.Text;
using System.Text.Json;
using EvrenDev.Application.Common.Mailing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EvrenDev.Infrastructure.Mailing;

public class AhasendMailService(IOptions<MailSettings> settings,
    ILogger<AhasendMailService> logger,
    IHttpClientFactory clientFactory) : IMailService
{
    private readonly MailSettings _settings = settings.Value;
    private readonly IHttpClientFactory _clientFactory = clientFactory;

    public async Task SendAsync(MailRequest request)
    {
        try
        {
            request.From ??= _settings.From;

            var jsonMailSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };

            var jsonContent = JsonSerializer.Serialize(request, jsonMailSerializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", _settings.ApiKey);

            var response = await httpClient.PostAsync(_settings.ApiUrl, content);

            logger.LogInformation("Sending email from {Email} with subject {Subject}", request?.From?.Email, request?.Content?.Subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", request.From?.Email);
            throw;
        }
    }
}
