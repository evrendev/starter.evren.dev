using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EvrenDev.Application.Common.Extensions;

public class ReCaptcha
{
    private readonly HttpClient _captchaClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReCaptcha> _logger;

    public ReCaptcha(HttpClient captchaClient,
        IConfiguration configuration,
        ILogger<ReCaptcha> logger)
    {
        _captchaClient = captchaClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> IsValid(string captcha)
    {
        try
        {
            var secretKey = _configuration.GetSection("Google:ReCaptcha:V3:SecretKey").Get<string>();
            var postTask = await _captchaClient.PostAsync($"?secret={secretKey}&response={captcha}", new StringContent(""));
            var result = await postTask.Content.ReadAsStringAsync();
            var resultObject = JObject.Parse(result);
            dynamic success = resultObject["success"] ?? false;
            return (bool)success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }
}
