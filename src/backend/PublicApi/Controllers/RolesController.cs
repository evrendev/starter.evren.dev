using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.Roles.Commands.CreateRole;
using EvrenDev.Application.Features.Roles.Commands.DeleteRole;
using EvrenDev.Application.Features.Roles.Commands.UpdateRole;
using EvrenDev.Application.Features.Roles.Model;
using EvrenDev.Application.Features.Roles.Queries.GetRoleById;
using EvrenDev.Application.Features.Roles.Queries.GetRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<RolesController> _localizer;

    public RolesController(IMediator mediator,
        IStringLocalizer<RolesController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpPost]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Create}")]
    public async Task<ActionResult<string>> Create(CreateRoleCommand command)
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

    [HttpGet]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Read}")]
    public async Task<ActionResult<List<RoleDto>>> GetAll([FromQuery] GetRolesQuery query)
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
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Read}")]
    public async Task<ActionResult<RoleDto>> GetById(string id)
    {
        try
        {
            var result = await _mediator.Send(new GetRoleByIdQuery { Id = id });
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
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Edit}")]
    public async Task<ActionResult<bool>> Update(string id, UpdateRoleCommand command)
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
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        try
        {
            var command = new DeleteRoleCommand { Id = id };
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
