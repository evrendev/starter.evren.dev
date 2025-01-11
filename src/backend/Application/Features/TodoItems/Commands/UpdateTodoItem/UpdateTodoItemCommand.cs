using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Note { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? Reminder { get; set; }
    public bool Done { get; set; }
}

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    private readonly IStringLocalizer<UpdateTodoItemCommandValidator> _localizer;
    public UpdateTodoItemCommandValidator(IStringLocalizer<UpdateTodoItemCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-items.update.id.required"]);

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(_localizer["api.todo-items.update.title.required"])
            .MaximumLength(200).WithMessage(_localizer["api.todo-items.update.title.maxlength"]);

        RuleFor(v => v.Priority)
            .IsInEnum().WithMessage(_localizer["api.todo-items.update.priority.invalid"]);
    }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id.ToString());
        }

        entity.Title = request.Title;
        entity.Note = request.Note;
        entity.Priority = request.Priority;
        entity.Reminder = request.Reminder;
        entity.Done = request.Done;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
