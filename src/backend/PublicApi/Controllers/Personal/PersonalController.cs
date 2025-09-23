using EvrenDev.Application.Auditing.Entities;
using EvrenDev.Application.Auditing.Queries.Get;
using EvrenDev.Application.Identity.Users.Commands.Update;
using EvrenDev.Application.Identity.Users.Entities;
using EvrenDev.Application.Identity.Users.Interfaces;
using EvrenDev.Application.Identity.Users.Password;

namespace EvrenDev.PublicApi.Controllers.Personal;

public class PersonalController(IUserService userService) : VersionNeutralApiController
{
    [HttpGet("profile")]
    [OpenApiOperation("Get profile details of currently logged in user.", "")]
    public async Task<ActionResult<UserDto>> GetProfileAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await userService.GetAsync(userId, cancellationToken));
    }

    [HttpPut("profile")]
    [OpenApiOperation("Update profile details of currently logged in user.", "")]
    public async Task<ApiResponse<UserDto>> UpdateProfileAsync(UpdateUserRequest request)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            return ApiResponse<UserDto>.Failure("Unauthorized");

        await userService.UpdateAsync(request, userId);

        var user = await userService.GetAsync(userId, CancellationToken.None);

        return ApiResponse<UserDto>.Success(user);
    }

    [HttpPut("change-password")]
    [OpenApiOperation("Change password of currently logged in user.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest model)
    {
        if (User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId))
            return ApiResponse<bool>.Failure("Unauthorized");

        await userService.ChangePasswordAsync(model, userId);
        return ApiResponse<bool>.Success(true);
    }

    [HttpGet("permissions")]
    [OpenApiOperation("Get permissions of currently logged in user.", "")]
    public async Task<ActionResult<List<string>>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        return User.GetUserId() is not { } userId || string.IsNullOrEmpty(userId)
            ? Unauthorized()
            : Ok(await userService.GetPermissionsAsync(userId, cancellationToken));
    }

    [HttpGet("logs")]
    [OpenApiOperation("Get audit logs of currently logged in user.", "")]
    public async Task<PaginationResponse<AuditDto>> GetPaginatedLogsAsync([FromQuery] PaginateAuditLogsFilter filter)
    {
        return await Mediator.Send(filter);
    }
}
