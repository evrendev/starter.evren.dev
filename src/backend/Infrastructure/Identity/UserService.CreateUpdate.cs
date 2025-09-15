using System.Security.Claims;
using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Mailing;
using EvrenDev.Application.Identity.Users.Commands.Create;
using EvrenDev.Application.Identity.Users.Commands.Update;
using EvrenDev.Domain.Common;
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
            throw new InternalServerException(localizer["Invalid objectId"]);
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
            throw new InternalServerException(string.Format(localizer["Username or Email not valid."]));
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new InternalServerException(string.Format(localizer["Username {0} is already taken."], username));
        }

        if (user is null)
        {
            user = await userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new InternalServerException(string.Format(localizer["Email {0} is already taken."], email));
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
            throw new InternalServerException(localizer["Validation Errors Occurred."], result.GetErrors(localizer));
        }

        return user;
    }

    public async Task<string> CreateAsync(CreateUserRequest request, string origin)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InternalServerException(localizer["Validation Errors Occurred."], result.GetErrors(localizer));
        }

        await userManager.AddToRoleAsync(user, ApiRoles.Basic);

        var messages = new List<string> { string.Format(localizer["User {0} Registered."], user.UserName) };

        if (_securitySettings.RequireConfirmedAccount && !string.IsNullOrEmpty(user.Email))
        {
            // send verification email
            var emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
            var eMailModel = new RegisterUserEmailModel
            {
                Email = user.Email,
                UserName = user.UserName,
                Url = emailVerificationUri
            };

            var htmlBody = templateService.GenerateEmailTemplate("email-confirmation", eMailModel);
            var textBody = localizer["Please confirm your account by clicking this link: {0}", emailVerificationUri];
            var subject = localizer["Confirm Registration"];

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
            messages.Add(localizer[$"Please check {user.Email} to verify your account!"]);
        }

        await events.PublishAsync(new ApplicationUserCreatedEvent(user.Id));

        return string.Join(Environment.NewLine, messages);
    }

    public async Task UpdateAsync(UpdateUserRequest request, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(localizer["User Not Found."]);

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
            throw new InternalServerException(localizer["Update profile failed"], result.GetErrors(localizer));
        }
    }
}
