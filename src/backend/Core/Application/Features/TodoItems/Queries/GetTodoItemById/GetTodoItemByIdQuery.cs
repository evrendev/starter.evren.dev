using EvrenDev.Application.Features.TodoItems.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoItems.Queries.GetTodoItemById;

public class GetTodoItemByIdQuery : IRequest<Result<TodoItemDto>>
{
    public Guid Id { get; set; }
}

public class GetTodoItemByIdQueryValidator : AbstractValidator<GetTodoItemByIdQuery>
{
    private readonly IStringLocalizer<GetTodoItemByIdQueryValidator> _localizer;

    public GetTodoItemByIdQueryValidator(IStringLocalizer<GetTodoItemByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-items.get.id.required"]);
    }
}

public class GetTodoItemByIdQueryHandler : IRequestHandler<GetTodoItemByIdQuery, Result<TodoItemDto>>
{
    private readonly ICatalogDbContext _context;

    public GetTodoItemByIdQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TodoItemDto>> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id.ToString());
        }

        var dto = new TodoItemDto
        {
            Id = entity.Id,
            ListId = entity.ListId,
            Title = entity.Title,
            Note = entity.Note,
            Priority = entity.Priority,
            Reminder = entity.Reminder,
            Done = entity.Done
        };

        return Result<TodoItemDto>.Success(dto);
    }
}
