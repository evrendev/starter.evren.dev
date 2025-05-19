using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<Result<Guid>>
{
    public string? Gender { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? JobTitle { get; init; }
    public string? Language { get; init; }
    public List<string> Permissions { get; init; } = [];
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
    }

    private async Task<bool> EmailExists(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
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

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Gender = Gender.From(request.Gender ?? Defaults.Gender),
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            JobTitle = request.JobTitle,
            Language = Language.From(request.Language ?? Defaults.Language),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password!);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<Guid>.Failure(errors);
        }

        if (request.Permissions.Any())
        {
            await _userManager.AddClaimsAsync(user, request.Permissions.Select(p => new System.Security.Claims.Claim("permission", p)));
        }

        return Result<Guid>.Success(user.Id);
    }
}
