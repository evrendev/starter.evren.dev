using EvrenDev.Application.Identity.Users.Commands.Create;
using EvrenDev.Application.Identity.Users.Commands.ToggleStatus;
using EvrenDev.Application.Identity.Users.Entities;
using EvrenDev.Application.Identity.Users.Interfaces;
using EvrenDev.Application.Identity.Users.Password;
using EvrenDev.Application.Identity.Users.Queries.Paginate;
using EvrenDev.Application.Identity.Users.Queries.UserRoles;
using EvrenDev.Infrastructure.Cors;

namespace EvrenDev.PublicApi.Controllers.Identity;

public class UsersController(IUserService userService, IConfiguration configuration) : VersionNeutralApiController
{
    [HttpGet("all")]
    [MustHavePermission(ApiAction.View, ApiResource.Users)]
    [OpenApiOperation("Get list of all users.", "")]
    public async Task<ApiResponse<List<UserDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var data = await userService.GetListAsync(cancellationToken);

        return ApiResponse<List<UserDto>>.Success(data);
    }

    [HttpGet]
    [MustHavePermission(ApiAction.View, ApiResource.Users)]
    [OpenApiOperation("Get paginated list of all users.", "")]
    public async Task<PaginationResponse<BasicUserDto>> GetPaginatedListAsync([FromQuery] PaginateUsersFilter filter, CancellationToken cancellationToken)
    {
        return await userService.PaginatedListAsync(filter, cancellationToken);
    }

    [HttpGet("{id}")]
    [MustHavePermission(ApiAction.View, ApiResource.Users)]
    [OpenApiOperation("Get a user's details.", "")]
    public async Task<ApiResponse<UserDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var data = await userService.GetAsync(id, cancellationToken);
        if (data == null)
            throw new NotFoundException($"User with ID '{id}' not found.");

        return ApiResponse<UserDto>.Success(data);
    }

    [HttpGet("{id}/roles")]
    [MustHavePermission(ApiAction.View, ApiResource.UserRoles)]
    [OpenApiOperation("Get a user's roles.", "")]
    public Task<List<UserRoleDto>> GetRolesAsync(string id, CancellationToken cancellationToken)
    {
        return userService.GetRolesAsync(id, cancellationToken);
    }

    [HttpPost("{id}/roles")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    [MustHavePermission(ApiAction.Update, ApiResource.UserRoles)]
    [OpenApiOperation("Update a user's assigned roles.", "")]
    public Task<string> AssignRolesAsync(string id, UserRolesRequest request, CancellationToken cancellationToken)
    {
        return userService.AssignRolesAsync(id, request, cancellationToken);
    }

    [HttpPost]
    [MustHavePermission(ApiAction.Create, ApiResource.Users)]
    [OpenApiOperation("Creates a new user.", "")]
    public Task<string> CreateAsync(CreateUserRequest request)
    {
        return userService.CreateAsync(request, GetOriginFromRequest());
    }

    // [HttpPost("self-register")]
    // [TenantIdHeader]
    // [AllowAnonymous]
    // [OpenApiOperation("Anonymous user creates a user.", "")]
    // [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    // public Task<string> SelfRegisterAsync(CreateUserRequest request)
    // {
    //     return userService.CreateAsync(request, GetOriginFromRequest());
    // }

    [HttpPost("{id}/toggle-status")]
    [MustHavePermission(ApiAction.Update, ApiResource.Users)]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    [OpenApiOperation("Toggle a user's active status.", "")]
    public async Task<ActionResult> ToggleStatusAsync(string id, ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        if (id != request.UserId)
        {
            return BadRequest();
        }

        await userService.ToggleStatusAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm email address for a user.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Search))]
    public Task<string> ConfirmEmailAsync([FromQuery] string tenant, [FromQuery] string userId, [FromQuery] string code, CancellationToken cancellationToken)
    {
        return userService.ConfirmEmailAsync(userId, code, tenant, cancellationToken);
    }

    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    [OpenApiOperation("Confirm phone number for a user.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Search))]
    public Task<string> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return userService.ConfirmPhoneNumberAsync(userId, code);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [TenantIdHeader]
    [OpenApiOperation("Request a pasword reset email for a user.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    public Task<string> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return userService.ForgotPasswordAsync(request, GetFrontendOriginFromRequest());
    }

    [HttpPost("reset-password")]
    [OpenApiOperation("Reset a user's password.", "")]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Register))]
    [AllowAnonymous]
    public Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return userService.ResetPasswordAsync(request);
    }

    private string GetOriginFromRequest() => $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";

    private string GetFrontendOriginFromRequest()
    {
        var corsSettings = configuration.GetSection(nameof(CorsSettings)).Get<CorsSettings>();

        return corsSettings?.Vue ?? "https://evren.dev";
    }
}
