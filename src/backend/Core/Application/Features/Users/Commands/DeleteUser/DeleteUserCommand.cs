using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand : IRequest<Result<bool>>
{
    public string Id { get; init; } = string.Empty;
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IStringLocalizer<DeleteUserCommandValidator> _localizer;

    public DeleteUserCommandValidator(
        IStringLocalizer<DeleteUserCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.users.delete.id.required"]);
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<DeleteUserCommandHandler> _localizer;

    public DeleteUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<DeleteUserCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
            return Result<bool>.Failure(_localizer["api.users.not-found"].Value);

        // Soft delete
        user.Deleted = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<bool>.Failure(errors);
        }

        return Result<bool>.Success(true);
    }
}
