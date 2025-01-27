namespace EvrenDev.Application.Features.TodoItems.Commands.DeleteTodoItem;
public class DeleteTodoItemCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
{
    private readonly IStringLocalizer<DeleteTodoItemCommandValidator> _localizer;
    public DeleteTodoItemCommandValidator(IStringLocalizer<DeleteTodoItemCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-items.delete.id.required"]);
    }
}

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, Result<bool>>
{
    private readonly ICatalogDbContext _context;

    public DeleteTodoItemCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoItem), request.Id.ToString());

        _context.TodoItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
