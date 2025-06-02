using System.Reflection;
using EvrenDev.Application.Version.Entities;

namespace EvrenDev.Application.Version.Queries.Get;

public class GetVersionInfoRequestHandler : IRequest<VersionInfoDto>
{
}

public class GetVersionInfoRequestHandler : IRequestHandler<GetVersionInfoRequest, VersionInfoDto>
{
    public Task<VersionInfoDto> Handle(GetVersionInfoRequest request, CancellationToken cancellationToken)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        return Task.FromResult(new VersionInfoDto { Version = version });
    }
}
