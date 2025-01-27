using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoLists.Commands.RestoreTodoList;

public class RestoreTodoListCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class RestoreTodoListCommandValidator : AbstractValidator<RestoreTodoListCommand>
{
    private readonly IStringLocalizer<RestoreTodoListCommandValidator> _localizer;

    public RestoreTodoListCommandValidator(IStringLocalizer<RestoreTodoListCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);
    }
}

public class RestoreTodoListCommandHandler : IRequestHandler<RestoreTodoListCommand, Result<bool>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStringLocalizer<RestoreTodoListCommandHandler> _localizer;

    public RestoreTodoListCommandHandler(
        ICatalogDbContext context,
        IStringLocalizer<RestoreTodoListCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(RestoreTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        if (!entity.Deleted)
            return Result<bool>.Failure(_localizer["api.todo-list.not-deleted"]);

        entity.Restore();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
