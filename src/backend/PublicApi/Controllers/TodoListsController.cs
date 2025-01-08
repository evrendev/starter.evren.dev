using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.TodoLists.Commands.CreateTodoList;
using EvrenDev.Application.Features.TodoLists.Commands.DeleteTodoList;
using EvrenDev.Application.Features.TodoLists.Commands.UpdateTodoList;
using EvrenDev.Application.Features.TodoLists.Models;
using EvrenDev.Application.Features.TodoLists.Queries.GetTodoListById;
using EvrenDev.Application.Features.TodoLists.Queries.GetTodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoListsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<TodoListsController> _localizer;

    public TodoListsController(
        IMediator mediator,
        IStringLocalizer<TodoListsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Read}")]
    public async Task<ActionResult<List<TodoListDto>>> GetAll()
    {
        try
        {
            var result = await _mediator.Send(new GetTodoListsQuery());
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
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Read}")]
    public async Task<ActionResult<TodoListDto>> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTodoListByIdQuery { Id = id });
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
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Create}")]
    public async Task<ActionResult<Guid>> Create(CreateTodoListCommand command)
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
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Edit}")]
    public async Task<ActionResult<bool>> Update(Guid id, UpdateTodoListCommand command)
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
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Delete}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        try
        {
            var command = new DeleteTodoListCommand { Id = id };
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
