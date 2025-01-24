using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.Audits.Models;
using EvrenDev.Application.Features.Audits.Queries.GetAuditById;
using EvrenDev.Application.Features.Audits.Queries.GetAudits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuditsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<AuditsController> _localizer;

    public AuditsController(IMediator mediator,
        IStringLocalizer<AuditsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Audits}.{Permissions.Read}")]
    public async Task<ActionResult<List<BasicAuditDto>>> GetAll([FromQuery] GetAuditsQuery query)
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

    [HttpGet("{id}")]
    [Authorize(Policy = $"{Modules.Audits}.{Permissions.Read}")]
    public async Task<ActionResult<FullAuditDto>> GetById(int id)
    {
        try
        {
            var result = await _mediator.Send(new GetAuditByIdQuery { Id = id });
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
