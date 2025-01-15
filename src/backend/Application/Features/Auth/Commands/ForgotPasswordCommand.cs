using Microsoft.AspNetCore.Identity;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Infrastructure.Services.Model;

namespace EvrenDev.Application.Features.Auth.Commands;

public class ForgotPasswordCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator(IStringLocalizer<ForgotPasswordCommand> localizer)
    {
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(localizer["api.auth.forgot-password.email.required"])
            .EmailAddress().WithMessage(localizer["api.auth.forgot-password.email.invalid"]);
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISendmailService _sendmailService;
    private readonly IStringLocalizer<ForgotPasswordCommandHandler> _localizer;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ISendmailService sendmailService,
        IStringLocalizer<ForgotPasswordCommandHandler> localizer)
    {
        _userManager = userManager;
        _sendmailService = sendmailService;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException(nameof(ApplicationUser), request.Email);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var emailRequest = new AhasendRequest
        {
            Recipients = new List<ContactDetails>
            {
                new ContactDetails
                {
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}"
                }
            },
            Content = new Content
            {
                Subject = _localizer["api.auth.forgot-password.email.subject"],
                TextBody = string.Format(_localizer["api.auth.forgot-password.email.body"], token)
            }
        };

        var result = await _sendmailService.SendEmailAsync(emailRequest);
        if (!result)
            return Result<string>.Failure(new[] { _localizer["api.auth.forgot-password.email-failed"].Value });

        return Result<string>.Success(_localizer["api.auth.forgot-password.email-sent"]);
    }
}
