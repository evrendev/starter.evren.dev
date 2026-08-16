using EvrenDev.Application.Identity.Users.Interfaces;

namespace EvrenDev.Application.Identity.Users.Commands.Create;

public class CreateUserRequest
{
    public List<string>? Roles { get; set; }
    public Gender Gender { get; set; } = default!;
    public Language Language { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    // Only honored on the anonymous self-register path — admin-created users
    // (isSelfRegister: false) always get a system-generated temporary
    // password regardless of this value (see UserService.CreateAsync).
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public string? PlaceOfBirth { get; set; }
}

public class CreateUserRequestValidator : CustomValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IUserService userService, IStringLocalizer<CreateUserRequestValidator> localizer)
    {
        RuleFor(u => u.Roles).NotEmpty().WithMessage(localizer["identity.users.roles.required"]);

        RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(localizer["identity.users.email.invalid"])
            .MustAsync(async (email, _) => !await userService.ExistsWithEmailAsync(email))
            .WithMessage((_, email) => string.Format(localizer["identity.users.email.registered"], email));

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            .MustAsync(async (phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!))
            .WithMessage((_, phone) => string.Format(localizer["identity.users.phone.registered"], phone))
            .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));

        RuleFor(p => p.FirstName)
            .NotEmpty()
            .MaximumLength(75);

        RuleFor(p => p.LastName)
            .NotEmpty()
            .MaximumLength(75);
    }
}
