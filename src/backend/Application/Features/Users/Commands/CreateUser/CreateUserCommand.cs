using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Result<string>>
{
    public Guid? TenantId { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Image { get; init; }
    public string? JobTitle { get; init; }
    public string Language { get; init; } = string.Empty;
    public List<string> Permissions { get; init; } = new();
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<CreateUserCommandValidator> _localizer;

    public CreateUserCommandValidator(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<CreateUserCommandValidator> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage(_localizer["api.users.create.email.required"])
            .EmailAddress().WithMessage(_localizer["api.users.create.email.invalid"])
            .MustAsync(async (email, _) => !await EmailExists(email))
                .WithMessage(_localizer["api.users.create.email.exists"]);

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage(_localizer["api.users.create.password.required"])
            .MinimumLength(8).WithMessage(_localizer["api.users.create.password.minlength"])
            .Matches("[A-Z]").WithMessage(_localizer["api.users.create.password.uppercase"])
            .Matches("[a-z]").WithMessage(_localizer["api.users.create.password.lowercase"])
            .Matches("[0-9]").WithMessage(_localizer["api.users.create.password.number"])
            .Matches("[^a-zA-Z0-9]").WithMessage(_localizer["api.users.create.password.special"]);

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage(_localizer["api.users.create.first-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.users.create.first-name.maxlength"]);

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage(_localizer["api.users.create.last-name.required"])
            .MaximumLength(100).WithMessage(_localizer["api.users.create.last-name.maxlength"]);

        RuleFor(v => v.TenantId)
            .NotEmpty().WithMessage(_localizer["api.users.create.tenant-id.required"]);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<CreateUserCommandHandler> _localizer;

    public CreateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<CreateUserCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            TenantId = request.TenantId,
            Gender = Gender.From(request.Gender),
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Image = request.Image,
            JobTitle = request.JobTitle,
            Language = Language.From(request.Language),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<string>.Failure(errors);
        }

        if (request.Permissions.Any())
        {
            await _userManager.AddClaimsAsync(user, request.Permissions.Select(p => new System.Security.Claims.Claim("permission", p)));
        }

        return Result<string>.Success(user.Id);
    }
}
