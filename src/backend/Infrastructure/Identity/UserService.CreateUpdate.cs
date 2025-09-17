using System.Security.Claims;
using System.Text;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Mailing;
using EvrenDev.Application.Identity.Users.Commands.Create;
using EvrenDev.Application.Identity.Users.Commands.Update;
using EvrenDev.Domain.Common.Events.Identity;
using EvrenDev.Domain.Identity;
using EvrenDev.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Web;

namespace EvrenDev.Infrastructure.Identity;

internal partial class UserService
{
    /// <summary>
    /// This is used when authenticating with AzureAd.
    /// The local user is retrieved using the objectidentifier claim present in the ClaimsPrincipal.
    /// If no such claim is found, an InternalServerException is thrown.
    /// If no user is found with that ObjectId, a new one is created and populated with the values from the ClaimsPrincipal.
    /// If a role claim is present in the principal, and the user is not yet in that roll, then the user is added to that role.
    /// </summary>
    public async Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        var objectId = principal.GetObjectId();
        if (string.IsNullOrWhiteSpace(objectId))
        {
            throw new InternalServerException(localizer["identity.users.create.invalidobjectid"]);
        }

        var user = await userManager.Users.Where(u => u.ObjectId == objectId).FirstOrDefaultAsync()
            ?? await CreateOrUpdateFromPrincipalAsync(principal);

        if (principal.FindFirstValue(ClaimTypes.Role) is string role &&
            await roleManager.RoleExistsAsync(role) &&
            !await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user.Id;
    }

    private async Task<ApplicationUser> CreateOrUpdateFromPrincipalAsync(ClaimsPrincipal principal)
    {
        var email = principal.FindFirstValue(ClaimTypes.Upn);
        var username = principal.GetDisplayName();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username))
        {
            throw new InternalServerException(localizer["identity.users.create.invalidcredentials"]);
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new InternalServerException(string.Format(localizer["identity.users.create.usernametaken"], username));
        }

        if (user is null)
        {
            user = await userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new InternalServerException(string.Format(localizer["identity.users.create.emailtaken"], email));
            }
        }

        IdentityResult? result;
        if (user is not null)
        {
            user.ObjectId = principal.GetObjectId();
            result = await userManager.UpdateAsync(user);

            await events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));
        }
        else
        {
            user = new ApplicationUser
            {
                ObjectId = principal.GetObjectId(),
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = principal.FindFirstValue(ClaimTypes.Surname),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            result = await userManager.CreateAsync(user);

            await events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));
        }

        if (!result.Succeeded)
        {
            throw new InternalServerException(localizer["identity.users.validation.error"], result.GetErrors(localizer));
        }

        return user;
    }

    public async Task<string> CreateAsync(CreateUserRequest request, string origin)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Birthday = request.Birthday,
            PlaceOfBirth = request.PlaceOfBirth,
            Language = request.Language,
            Gender = request.Gender,
            IsActive = false
        };

        var temporaryPassword = GenerateRandomPassword();

        var result = await userManager.CreateAsync(user, temporaryPassword);
        if (!result.Succeeded)
        {
            throw new InternalServerException(localizer["identity.users.validation.error"], result.GetErrors(localizer));
        }

        await userManager.AddToRoleAsync(user, ApiRoles.Basic);

        var messages = new List<string> { string.Format(localizer["identity.users.create.registered"], user.UserName) };
        
        if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))
        {
            // send verification email
            var emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
            var eMailModel = new RegisterUserEmailModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Password = temporaryPassword,
                Email = user.Email,
                Url = emailVerificationUri,

            };

            var htmlBody = templateService.GenerateEmailTemplate("email-confirmation", eMailModel);
            var textBody = localizer["identity.users.confirm.link", emailVerificationUri];
            var subject = localizer["identity.auth.confirmregistration"];

            var recipients = new List<Contact>
            {
                new(user.Email, $"{user.FirstName} {user.LastName}")
            };

            var content = new Content
            {
                Subject = subject,
                TextBody = textBody,
                HtmlBody = htmlBody,
            };

            var mailRequest = new MailRequest(
                content,
                recipients
            );

            jobService.Enqueue(() => mailService.SendAsync(mailRequest));
            messages.Add(localizer["identity.users.confirm.check", user.Email]);
        }

        await events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

        return string.Join(Environment.NewLine, messages);
    }

    public async Task<string> UpdateAsync(UpdateUserRequest request, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(localizer["identity.users.notfound"]);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.Birthday = request.Birthday;
        user.PlaceOfBirth = request.PlaceOfBirth;
        user.Language = request.Language;
        user.Gender = request.Gender;

        var phoneNumber = await userManager.GetPhoneNumberAsync(user);
        if (request.PhoneNumber != phoneNumber)
        {
            await userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
        }

        var email = await userManager.GetEmailAsync(user);

        if (request.Email != email)
        {
            await userManager.SetEmailAsync(user, request.Email);
        }

        var result = await userManager.UpdateAsync(user);

        await signInManager.RefreshSignInAsync(user);

        await events.PublishAsync(new ApplicationUserUpdatedEvent(user.Id));

        if (!result.Succeeded)
        {
            throw new InternalServerException(localizer["identity.users.update.failed"], result.GetErrors(localizer));
        }

        return user.Id;
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        var result = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }
        return result.ToString();
    }
}
