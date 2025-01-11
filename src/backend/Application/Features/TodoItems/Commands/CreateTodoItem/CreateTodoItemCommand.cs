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
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
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
