namespace EvrenDev.Application.Catalog.Absences.Commands.UpdateAbsence;

public class UpdateAbsenceCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? CalendarId { get; set; }
}

public class UpdateAbsenceCommandValidator : AbstractValidator<UpdateAbsenceCommand>
{
    private readonly IStringLocalizer<UpdateAbsenceCommandValidator> _localizer;

    public UpdateAbsenceCommandValidator(IStringLocalizer<UpdateAbsenceCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.absence.update.id.required"]);

        RuleFor(v => v.Start)
            .NotEmpty().WithMessage(_localizer["api.absence.create.startdate.required"])
            .LessThan(v => v.End).WithMessage(_localizer["api.absence.create.startdate.lessThanEndDate"]);

        RuleFor(v => v.End)
            .NotEmpty().WithMessage(_localizer["api.absence.create.enddate.required"]);

        RuleFor(v => v.Employee)
            .NotEmpty().WithMessage(_localizer["api.absence.create.employee.required"])
            .MaximumLength(100).WithMessage(_localizer["api.absence.create.employee.maxlength"]);

        RuleFor(v => v.CalendarId)
            .NotEmpty().WithMessage(_localizer["api.absence.create.calendar.required"])
            .MaximumLength(100).WithMessage(_localizer["api.absence.create.calendar.maxlength"]);

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage(_localizer["api.absence.create.description.maxlength"]);

        RuleFor(v => v.Location)
            .MaximumLength(50).WithMessage(_localizer["api.absence.create.location.maxlength"]);
    }
}

public class UpdateAbsenceCommandHandler : IRequestHandler<UpdateAbsenceCommand, Result<bool>>
{
    private readonly ICatalogDbContext _context;
    private readonly IStringLocalizer<UpdateAbsenceCommandHandler> _localizer;

    public UpdateAbsenceCommandHandler(
        ICatalogDbContext context,
        IStringLocalizer<UpdateAbsenceCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateAbsenceCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Absences.FindAsync([request.Id], cancellationToken);

        if (entity == null)
            throw new NotFoundException(nameof(Absence), request.Id.ToString());

        entity.StartDate = request.Start;
        entity.EndDate = request.End;
        entity.Description = request.Description;
        entity.Location = request.Location;
        entity.Employee = request.Employee;
        entity.CalendarId = request.CalendarId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
