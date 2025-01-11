using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Features.TodoItems.Commands.CreateTodoItem;
using EvrenDev.Application.Features.TodoItems.Commands.DeleteTodoItem;
using EvrenDev.Application.Features.TodoItems.Commands.UpdateTodoItem;
using EvrenDev.Application.Features.TodoItems.Models;
using EvrenDev.Application.Features.TodoItems.Queries.GetTodoItemById;
using EvrenDev.Application.Features.TodoItems.Queries.GetTodoItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EvrenDev.Shared.Constants.Policies;

namespace EvrenDev.PublicApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<TodoItemsController> _localizer;

    public TodoItemsController(
        IMediator mediator,
        IStringLocalizer<TodoItemsController> localizer)
    {
        _mediator = mediator;
        _localizer = localizer;
    }

    [HttpGet]
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Read}")]
    public async Task<ActionResult<List<TodoItemDto>>> GetAll([FromQuery] GetTodoItemsQuery query)
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
    [Authorize(Policy = $"{Modules.Todos}.{Permissions.Read}")]
    public async Task<ActionResult<TodoItemDto>> GetById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetTodoItemByIdQuery { Id = id });
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
    public async Task<ActionResult<Guid>> Create(CreateTodoItemCommand command)
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
    public async Task<ActionResult<bool>> Update(Guid id, UpdateTodoItemCommand command)
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
            var command = new DeleteTodoItemCommand { Id = id };
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
