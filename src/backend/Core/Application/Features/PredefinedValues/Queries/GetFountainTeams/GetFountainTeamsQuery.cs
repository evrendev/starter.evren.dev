namespace EvrenDev.Application.Features.PredefinedValues.Queries.GetMediaStatuses;
public class GetMediaStatusesQuery : IRequest<Result<IEnumerable<MediaStatus>?>>
{
}

public class GetMediaStatusesQueryHandler : IRequestHandler<GetMediaStatusesQuery, Result<IEnumerable<MediaStatus>?>>
{
    private readonly IStringLocalizer<GetMediaStatusesQueryHandler> _localizer;
    public GetMediaStatusesQueryHandler(IStringLocalizer<GetMediaStatusesQueryHandler> localizer)
    {
        _localizer = localizer;
    }

    public async Task<Result<IEnumerable<MediaStatus>?>> Handle(GetMediaStatusesQuery request, CancellationToken cancellationToken)
    {
        var values = MediaStatus.ToList;

        return await Task.FromResult(Result<IEnumerable<MediaStatus>?>.Success(values));
    }
}
