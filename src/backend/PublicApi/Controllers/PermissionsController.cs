using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.Permissions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<PermissionsController> _localizer;

    public PermissionsController(IMediator mediator,
        IStringLocalizer<PermissionsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<string[]>> GetAll([FromQuery] GetAllPermissionsQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);

            return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Error = true,
                message = _localizer["api.validations.failed"].Value,
                Errors = ex.Errors.Select(x => new
                {
                    key = x.Key.ToLowerInvariant(),
                    value = x.Value[0]
                }).ToList()
            });
        }
    }
}
