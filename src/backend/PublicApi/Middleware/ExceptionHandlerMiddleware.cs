using System.Net;
using System.Text.Json;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Localization;

namespace EvrenDev.PublicApi.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IStringLocalizer<ExceptionHandlerMiddleware> _localizer;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IStringLocalizer<ExceptionHandlerMiddleware> localizer)
    {
        _next = Guard.Against.Null(next);
        _logger = Guard.Against.Null(logger);
        _localizer = Guard.Against.Null(localizer);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                // Add other exception types here as needed
                _ => (int)HttpStatusCode.InternalServerError
            };

            var localizedMessage = error switch
            {
                NotFoundException => _localizer["api.item-not-found"],
                _ => _localizer["api.validations.failed"]
            };

            var result = JsonSerializer.Serialize(new
            {
                message = localizedMessage.Value,
                statusCode = response.StatusCode
            });

            await response.WriteAsync(result);
        }
    }
}
