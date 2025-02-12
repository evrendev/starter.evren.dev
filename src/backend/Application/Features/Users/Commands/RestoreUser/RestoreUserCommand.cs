using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Users.Commands.RestoreUser;

public record RestoreUserCommand : IRequest<Result<bool>>
{
    public Guid? Id { get; set; }
}

public class RestoreUserCommandValidator : AbstractValidator<RestoreUserCommand>
{
    private readonly IStringLocalizer<RestoreUserCommandValidator> _localizer;

    public RestoreUserCommandValidator(
        IStringLocalizer<RestoreUserCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.users.restore.id.required"]);
    }
}

public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<RestoreUserCommandHandler> _localizer;

    public RestoreUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<RestoreUserCommandHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsQueryable().IgnoreQueryFilters();
        var user = await query.SingleOrDefaultAsync(u => u.Id == request.Id);

        if (user == null)
            return Result<bool>.Failure(_localizer["api.users.not-found"].Value);

        user.Restore();

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<bool>.Failure(errors);
        }

        return Result<bool>.Success(true);
    }
}
