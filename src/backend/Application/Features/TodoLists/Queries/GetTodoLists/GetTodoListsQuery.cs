using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.TodoLists.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoLists.Queries.GetTodoLists;

public class GetTodoListsQuery : IRequest<Result<List<TodoListDto>>>
{
}

public class GetTodoListsQueryHandler : IRequestHandler<GetTodoListsQuery, Result<List<TodoListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoListsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TodoListDto>>> Handle(GetTodoListsQuery request, CancellationToken cancellationToken)
    {
        var lists = await _context.TodoLists
            .AsNoTracking()
            .Select(list => new TodoListDto
            {
                Id = list.Id,
                Title = list.Title,
                Colour = list.Colour
            })
            .ToListAsync(cancellationToken);

        return Result<List<TodoListDto>>.Success(lists);
    }
}
