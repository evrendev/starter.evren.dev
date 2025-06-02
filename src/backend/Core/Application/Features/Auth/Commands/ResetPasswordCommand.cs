using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Auth.Commands;

public class ResetPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(IStringLocalizer<ResetPasswordCommand> localizer)
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(localizer["api.auth.reset-password.email.required"])
            .EmailAddress().WithMessage(localizer["api.auth.reset-password.email.invalid"]);

        RuleFor(v => v.Token)
            .NotEmpty().WithMessage(localizer["api.auth.reset-password.token.required"]);

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage(localizer["api.auth.reset-password.password.required"])
            .MinimumLength(8).WithMessage(localizer["api.auth.reset-password.password.minlength"])
            .Matches("[A-Z]").WithMessage(localizer["api.auth.reset-password.password.uppercase"])
            .Matches("[a-z]").WithMessage(localizer["api.auth.reset-password.password.lowercase"])
            .Matches("[0-9]").WithMessage(localizer["api.auth.reset-password.password.number"])
            .Matches("[^a-zA-Z0-9]").WithMessage(localizer["api.auth.reset-password.password.special"]);
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<ResetPasswordCommandHandler> _localizer;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<ResetPasswordCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException(nameof(ApplicationUser), request.Email);

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => _localizer[e.Description]).ToArray();
            return Result<string>.Failure(errors.Select(e => e.Value).ToArray());
        }

        return Result<string>.Success(_localizer["api.auth.reset-password.success"]);
    }
}
