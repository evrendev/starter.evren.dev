namespace EvrenDev.Application.Features.PredefinedValues.Queries.GetFountaionTeams;
public class GetFountaionTeamsQuery : IRequest<Result<IEnumerable<FountaionTeam>?>>
{
}

public class GetFountaionTeamsQueryHandler : IRequestHandler<GetFountaionTeamsQuery, Result<IEnumerable<FountaionTeam>?>>
{
    private readonly IStringLocalizer<GetFountaionTeamsQueryHandler> _localizer;
    public GetFountaionTeamsQueryHandler(IStringLocalizer<GetFountaionTeamsQueryHandler> localizer)
    {
        _localizer = localizer;
    }

    public async Task<Result<IEnumerable<FountaionTeam>?>> Handle(GetFountaionTeamsQuery request, CancellationToken cancellationToken)
    {
        var values = FountaionTeam.ToList;

        return await Task.FromResult(Result<IEnumerable<FountaionTeam>?>.Success(values));
    }
}
