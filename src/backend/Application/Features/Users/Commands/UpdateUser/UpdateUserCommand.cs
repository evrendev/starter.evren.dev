using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand : IRequest<Result<bool>>
{
    public string Id { get; init; } = string.Empty;
    public string? TenantId { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Image { get; init; }
    public string? JobTitle { get; init; }
    public string Language { get; init; } = string.Empty;
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UpdateUserCommandValidator> _localizer;

    public UpdateUserCommandValidator(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<UpdateUserCommandValidator> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.users.update.id.required"]);

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_localizer["api.users.update.email.required"])
            .EmailAddress().WithMessage(_localizer["api.users.update.email.invalid"])
            .MustAsync(async (model, email, _) => await EmailBelongsToUser(model.Id, email))
                .WithMessage(_localizer["api.users.update.email.exists"]);

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage(_localizer["api.users.update.first-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.users.update.first-name.maxlength"]);

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage(_localizer["api.users.update.last-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.users.update.last-name.maxlength"]);

        RuleFor(v => v.TenantId)
            .NotEmpty().WithMessage(_localizer["api.users.update.tenant-id.required"]);
    }

    private async Task<bool> EmailBelongsToUser(string userId, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null || user.Id == userId;
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<UpdateUserCommandHandler> _localizer;

    public UpdateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<UpdateUserCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
            return Result<bool>.Failure(_localizer["api.users.not-found"].Value);

        user.TenantId = request.TenantId;
        user.Gender = Gender.From(request.Gender);
        user.UserName = request.Email;
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Image = request.Image;
        user.JobTitle = request.JobTitle;
        user.Language = Language.From(request.Language);

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<bool>.Failure(errors);
        }

        return Result<bool>.Success(true);
    }
}
