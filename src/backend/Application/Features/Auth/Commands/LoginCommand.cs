using System.Text.Json.Serialization;
using EvrenDev.Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;

namespace EvrenDev.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    [JsonPropertyName("rememberMe")]
    public bool RememberMe { get; set; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IStringLocalizer<LoginCommand> localizer)
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(localizer["api.auth.login.email.required"])
            .EmailAddress().WithMessage(localizer["api.auth.login.email.invalid"]);

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage(localizer["api.auth.login.password.required"]);
    }
}

public class LoginCommandResponse
{
    public AuthResponse? AuthResponse { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string? UserId { get; set; }
}
