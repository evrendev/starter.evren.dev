using EvrenDev.Application.Dashboard;

namespace EvrenDev.PublicApi.Controllers.Dashboard;

public class DashboardController : VersionedApiController
{
    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Dashboard)]
    [OpenApiOperation("Get statistics for the dashboard.", "")]
    public async Task<ApiResponse<StatsDto>> GetAsync()
    {
        var data = await Mediator.Send(new GetStatsRequest());

        return ApiResponse<StatsDto>.Success(data);
    }
}
