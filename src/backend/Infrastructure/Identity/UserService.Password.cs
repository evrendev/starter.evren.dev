using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Mailing;
using EvrenDev.Application.Identity.Users.Password;
using Microsoft.AspNetCore.WebUtilities;

namespace EvrenDev.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        EnsureValidTenant();

        var user = await userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new InternalServerException(localizer["identity.password.reset.notfound"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "auth/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var queryParams = new Dictionary<string, string?>
        {
            {"email", user.Email},
            {"token", code}
        };
        var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), queryParams);
        var recipients = new List<Contact>
        {
            new(user.Email, $"{user.FirstName} {user.LastName}")
        };

        var content = new Content
        {
            Subject = localizer["identity.password.reset.subject"],
            TextBody = localizer["identity.password.reset.text.body", code, passwordResetUrl],
            HtmlBody = localizer["identity.password.reset.html.body", passwordResetUrl, localizer["identity.password.reset.button"]],
        };

        var mailRequest = new MailRequest(
            content,
            recipients
        );

        jobService.Enqueue(() => mailService.SendAsync(mailRequest));

        return localizer["identity.password.reset.success"];
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email?.Normalize());

        // Don't reveal that the user does not exist
        _ = user ?? throw new InternalServerException(localizer["An Error has occurred!"]);

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

        return result.Succeeded
            ? localizer["identity.password.reset.success"]
            : throw new InternalServerException(localizer["An Error has occurred!"]);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(localizer["identity.user.notfound"]);

        var result = await userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!result.Succeeded)
        {
            throw new InternalServerException(localizer["identity.password.change.failed"], result.GetErrors(localizer));
        }
    }
}
