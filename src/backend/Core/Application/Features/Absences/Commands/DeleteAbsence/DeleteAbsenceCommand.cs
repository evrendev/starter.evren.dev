namespace EvrenDev.Application.Features.Absences.Commands.DeleteAbsence;

public class DeleteAbsenceCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteAbsenceCommandValidator : AbstractValidator<DeleteAbsenceCommand>
{
    private readonly IStringLocalizer<DeleteAbsenceCommandValidator> _localizer;

    public DeleteAbsenceCommandValidator(IStringLocalizer<DeleteAbsenceCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.todo-lists.delete.id.required"]);
    }
}

public class DeleteAbsenceCommandHandler : IRequestHandler<DeleteAbsenceCommand, Result<bool>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStringLocalizer<DeleteAbsenceCommandHandler> _localizer;

    public DeleteAbsenceCommandHandler(
        ICatalogDbContext context,
        IStringLocalizer<DeleteAbsenceCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteAbsenceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Absences.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Absence), request.Id.ToString());

        _context.Absences.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
