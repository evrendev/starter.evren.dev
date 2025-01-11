namespace EvrenDev.Application.Features.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Colour { get; set; }
}

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IStringLocalizer<UpdateTodoListCommandValidator> _localizer;

    public UpdateTodoListCommandValidator(IStringLocalizer<UpdateTodoListCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.update.id.required"]);

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.update.title.required"])
            .MaximumLength(200).WithMessage(_localizer["api.todo-lists.update.title.maxlength"]);

        RuleFor(v => v.Colour)
            .Must(BeValidColour).WithMessage(_localizer["api.todo-lists.update.colour.invalid"])
            .When(v => !string.IsNullOrWhiteSpace(v.Colour));
    }

    private static bool BeValidColour(string? colour)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(colour))
                return true;

            _ = Colour.From(colour);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IStringLocalizer<UpdateTodoListCommandHandler> _localizer;

    public UpdateTodoListCommandHandler(
        IApplicationDbContext context,
        IStringLocalizer<UpdateTodoListCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoLists.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(TodoList), request.Id.ToString());

        entity.Title = request.Title;
        entity.Colour = string.IsNullOrWhiteSpace(request.Colour) ? Colour.White : Colour.From(request.Colour);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
