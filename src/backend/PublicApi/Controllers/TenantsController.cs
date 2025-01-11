using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.Tenants.Commands.CreateTenant;
using EvrenDev.Application.Features.Tenants.Commands.DeleteTenant;
using EvrenDev.Application.Features.Tenants.Commands.UpdateTenant;
using EvrenDev.Application.Features.Tenants.Models;
using EvrenDev.Application.Features.Tenants.Queries.GetTenantById;
using EvrenDev.Application.Features.Tenants.Queries.GetTenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<TenantsController> _localizer;

    public TenantsController(
        IMediator mediator,
        IStringLocalizer<TenantsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Tenants}.{Permissions.Read}")]
    public async Task<ActionResult<List<TenantDto>>> GetAll([FromQuery] GetTenantsQuery query)
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
    [Authorize(Policy = $"{Modules.Tenants}.{Permissions.Read}")]
    public async Task<ActionResult<TenantDto>> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTenantByIdQuery { Id = id });
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
    [Authorize(Policy = $"{Modules.Tenants}.{Permissions.Create}")]
    public async Task<ActionResult<Guid>> Create(CreateTenantCommand command)
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
    [Authorize(Policy = $"{Modules.Tenants}.{Permissions.Edit}")]
    public async Task<ActionResult<bool>> Update(Guid id, UpdateTenantCommand command)
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
    [Authorize(Policy = $"{Modules.Tenants}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteTenantCommand { Id = id };
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
