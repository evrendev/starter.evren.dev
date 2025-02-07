using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EvrenDev.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public string Id { get; init; } = string.Empty;
}

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IStringLocalizer<GetUserByIdQueryValidator> _localizer;

    public GetUserByIdQueryValidator(
        IStringLocalizer<GetUserByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.users.get.id.required"]);
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStringLocalizer<GetUserByIdQueryHandler> _localizer;

    public GetUserByIdQueryHandler(
        UserManager<ApplicationUser> userManager,
        IStringLocalizer<GetUserByIdQueryHandler> localizer)
    {
        _userManager = userManager;
        _localizer = localizer;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
            return Result<UserDto>.Failure(_localizer["api.users.not-found"].Value);

        var claims = await _userManager.GetClaimsAsync(user);
        var permissions = claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList();

        var userDto = new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Gender = user.Gender?.Code,
            Email = user.Email!,
            FirstName = user.FirstName!,
            LastName = user.LastName!,
            FullName = user.FullName,
            Image = user.Image ?? string.Empty,
            JobTitle = user.JobTitle ?? string.Empty,
            Language = user.Language?.Code,
            Permissions = permissions,
            TwoFactorEnabled = user.TwoFactorEnabled
        };

        return Result<UserDto>.Success(userDto);
    }
}
