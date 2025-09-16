using EvrenDev.Application.Identity.Users.Interfaces;

namespace EvrenDev.Application.Identity.Users.Commands.Update;

public class UpdateUserRequest
{
    public Gender? Gender { get; set; }
    public Language? Language { get; set; }
    public string Id { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birthday { get; set; }
    public string? PlaceOfBirth { get; set; }
}

public class UpdateUserRequestValidator : CustomValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IUserService userService, IStringLocalizer<UpdateUserRequestValidator> localizer)
    {
        RuleFor(p => p.Id)
            .NotEmpty();

        RuleFor(p => p.FirstName)
            .NotEmpty()
            .MaximumLength(75);

        RuleFor(p => p.LastName)
            .NotEmpty()
            .MaximumLength(75);

        RuleFor(p => p.Email)
            .NotEmpty()
            .EmailAddress()
                .WithMessage(localizer["Invalid Email Address."])
            .MustAsync(async (user, email, _) => !await userService.ExistsWithEmailAsync(email, user.Id))
                .WithMessage((_, email) => string.Format(localizer["Email {0} is already registered."], email));

        RuleFor(u => u.PhoneNumber).Cascade(CascadeMode.Stop)
            .MustAsync(async (user, phone, _) => !await userService.ExistsWithPhoneNumberAsync(phone!, user.Id))
                .WithMessage((_, phone) => string.Format(localizer["Phone number {0} is already registered."], phone))
                .Unless(u => string.IsNullOrWhiteSpace(u.PhoneNumber));
    }
}
