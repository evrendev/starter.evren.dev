using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.Absences.Commands.CreateAbsence;
using EvrenDev.Application.Features.Absences.Commands.DeleteAbsence;
using EvrenDev.Application.Features.Absences.Commands.UpdateAbsence;
using EvrenDev.Application.Features.Absences.Models;
using EvrenDev.Application.Features.Absences.Queries.GetAbsences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/absences")]
public class AbsencesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<AbsencesController> _localizer;

    public AbsencesController(
        IMediator mediator,
        IStringLocalizer<AbsencesController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Absences}.{Permissions.Read}")]
    public async Task<ActionResult<List<AbsenceDto>>> GetAll(GetAbsencesQuery? query)
    {
        try
        {
            var result = await _mediator.Send(query ?? new GetAbsencesQuery());
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

    [HttpPost]
    [Authorize(Policy = $"{Modules.Absences}.{Permissions.Create}")]
    public async Task<ActionResult<Guid>> Create(CreateAbsenceCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
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

    [HttpPut("{id}")]
    [Authorize(Policy = $"{Modules.Absences}.{Permissions.Edit}")]
    public async Task<ActionResult<bool>> Update(Guid id, UpdateAbsenceCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        try
        {
            var result = await _mediator.Send(command);
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

    [HttpDelete("{id}")]
    [Authorize(Policy = $"{Modules.Absences}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteAbsenceCommand { Id = id };
            var result = await _mediator.Send(command);
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
