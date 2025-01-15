using EvrenDev.Application.Features.TodoLists.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoLists.Queries.GetTodoListById;

public class GetTodoListByIdQuery : IRequest<Result<TodoListDto>>
{
    public Guid Id { get; set; }
}

public class GetTodoListByIdQueryValidator : AbstractValidator<GetTodoListByIdQuery>
{
    private readonly IStringLocalizer<GetTodoListByIdQueryValidator> _localizer;

    public GetTodoListByIdQueryValidator(IStringLocalizer<GetTodoListByIdQueryValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.get.id.required"]);
    }
}

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, Result<TodoListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<GetTodoListByIdQueryHandler> _localizer;

    public GetTodoListByIdQueryHandler(
        IApplicationDbContext context,
        IStringLocalizer<GetTodoListByIdQueryHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<TodoListDto>> Handle(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var dto = new TodoListDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Colour = entity.Colour,
            Items = entity.Items.Select(x => new BasicTodoItemDto
            {
                Title = x.Title,
                Note = x.Note,
                Priority = x.Priority,
                Reminder = x.Reminder,
                Done = x.Done
            }).ToList()
        };

        return Result<TodoListDto>.Success(dto);
    }
}
