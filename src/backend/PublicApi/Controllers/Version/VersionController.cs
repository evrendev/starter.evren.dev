using EvrenDev.Application.Version.Entities;
using EvrenDev.Application.Version.Queries.Get;

namespace EvrenDev.Host.Controllers.Version;

public class VersionController : VersionNeutralApiController
{
    [HttpGet("info")]
    [AllowAnonymous]
    [OpenApiOperation("Get application information.", "")]
    public Task<VersionInfoDto> GetAsync()
    {
        return Mediator.Send(new GetVersionInfoRequest());
    }
}
