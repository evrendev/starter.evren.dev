using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.PredefinedValues.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/predefined-values")]
public class PredefinedValuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<PredefinedValuesController> _localizer;

    public PredefinedValuesController(IMediator mediator,
        IStringLocalizer<PredefinedValuesController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<ActionResult<PredefinedValuesVm>> GetAll([FromQuery] GetAllPredefinedValuesQuery query)
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
