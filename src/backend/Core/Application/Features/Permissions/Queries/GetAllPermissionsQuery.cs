using EvrenDev.Shared.Constants;

namespace EvrenDev.Application.Features.Permissions.Queries;
public class GetAllPermissionsQuery : IRequest<Result<string[]>>
{
}

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<string[]>>
{
    private readonly IStringLocalizer<GetAllPermissionsQueryHandler> _localizer;
    public GetAllPermissionsQueryHandler(IStringLocalizer<GetAllPermissionsQueryHandler> localizer)
    {
        _localizer = localizer;
    }

    public async Task<Result<string[]>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var allPermissions = Policies.AllModulesWithPermissions;

        return await Task.FromResult(Result<string[]>.Success(allPermissions));
    }
}
