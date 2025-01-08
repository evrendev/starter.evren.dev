using Ardalis.GuardClauses;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Application.Common.Models;
using EvrenDev.Application.Features.TodoLists.Models;
using EvrenDev.Domain.Entities.Catalog;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

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
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        var dto = new TodoListDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Colour = entity.Colour
        };

        return Result<TodoListDto>.Success(dto);
    }
}
