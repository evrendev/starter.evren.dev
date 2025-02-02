using EvrenDev.Application.Common.Exceptions;

namespace EvrenDev.Application.Features.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommand : IRequest<Result<Guid>>
{
    public Guid ListId { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? Reminder { get; set; }
}

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    private readonly IStringLocalizer<CreateTodoItemCommandValidator> _localizer;
    public CreateTodoItemCommandValidator(IStringLocalizer<CreateTodoItemCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.ListId)
            .NotEmpty().WithMessage(_localizer["api.todo-items.create.listId.required"]);

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(_localizer["api.todo-items.create.title.required"])
            .MaximumLength(200).WithMessage(_localizer["api.todo-items.create.title.maxlength"]);

        RuleFor(v => v.Priority)
            .IsInEnum().WithMessage(_localizer["api.todo-items.create.priority.invalid"]);
    }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, Result<Guid>>
{
    private readonly ICatalogDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<CreateTodoItemCommandHandler> _localizer;

    public CreateTodoItemCommandHandler(ICatalogDbContext context,
        ICurrentUser currentUser,
        IStringLocalizer<CreateTodoItemCommandHandler> localizer)
    {
        _context = context;
        _currentUser = currentUser;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var list = await _context.TodoLists.FindAsync(new object[] { request.ListId }, cancellationToken);
        if (list == null)
            throw new NotFoundException(nameof(TodoList), request.ListId.ToString());

        if (list.Creator != _currentUser.Email)
            throw new ForbiddenException(nameof(TodoList), _localizer["api.todo-items.create.forbidden"]);

        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Note = request.Note,
            Priority = request.Priority,
            Reminder = request.Reminder
        };

        _context.TodoItems.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
