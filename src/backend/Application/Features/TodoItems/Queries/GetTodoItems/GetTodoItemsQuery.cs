using EvrenDev.Application.Features.TodoItems.Models;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoItems.Queries.GetTodoItems;

public class GetTodoItemsQuery : IRequest<Result<List<TodoItemDto>>>
{
    public Guid? ListId { get; set; }
    public bool ShowDeletedItems { get; set; } = false;
}

public class GetTodoItemsQueryHandler : IRequestHandler<GetTodoItemsQuery, Result<List<TodoItemDto>>>
{
    private readonly ICatalogDbContext _context;

    public GetTodoItemsQueryHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TodoItemDto>>> Handle(GetTodoItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TodoItems.AsQueryable();

        if (request.ShowDeletedItems)
            query = query.IgnoreQueryFilters();

        if (request.ListId.HasValue)
        {
            query = query.Where(x => x.ListId == request.ListId.Value);
        }

        var items = await query
            .OrderBy(x => x.Title)
            .Select(x => new TodoItemDto
            {
                Id = x.Id,
                ListId = x.ListId,
                Title = x.Title,
                Note = x.Note,
                Priority = x.Priority,
                Reminder = x.Reminder,
                Done = x.Done
            })
            .ToListAsync(cancellationToken);

        return Result<List<TodoItemDto>>.Success(items);
    }
}
