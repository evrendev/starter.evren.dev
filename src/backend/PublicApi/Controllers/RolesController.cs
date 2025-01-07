using EvrenDev.Application.Features.Roles.Commands.CreateRole;
using EvrenDev.Application.Features.Roles.Commands.DeleteRole;
using EvrenDev.Application.Features.Roles.Commands.UpdateRole;
using EvrenDev.Application.Features.Roles.Model;
using EvrenDev.Application.Features.Roles.Queries.GetRoleById;
using EvrenDev.Application.Features.Roles.Queries.GetRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Create}")]
    public async Task<ActionResult<string>> Create(CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Read}")]
    public async Task<ActionResult<List<RoleDto>>> GetAll([FromQuery] GetRolesQuery query)
    {
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Read}")]
    public async Task<ActionResult<RoleDto>> GetById(string id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery { Id = id });
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Edit}")]
    public async Task<ActionResult<bool>> Update(string id, UpdateRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = $"{Modules.Roles}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> Delete(string id)
    {
        var command = new DeleteRoleCommand { Id = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
