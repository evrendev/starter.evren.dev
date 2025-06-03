using EvrenDev.Application.Version.Entities;
using EvrenDev.Application.Version.Queries.Get;

namespace EvrenDev.PublicApi.Controllers.Version;

public class VersionController : VersionNeutralApiController
{
    [HttpGet]
    [AllowAnonymous]
    [OpenApiOperation("Get application information.", "")]
    public Task<VersionInfoDto> GetAsync()
    {
        return Mediator.Send(new GetVersionInfoRequest());
    }
}
