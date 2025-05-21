namespace EvrenDev.Application.Features.Absences.Commands.CreateAbsence;

public class CreateAbsenceCommand : IRequest<Result<Guid>>
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? CalendarId { get; set; }
}

public class CreateAbsenceCommandValidator : AbstractValidator<CreateAbsenceCommand>
{
    private readonly IStringLocalizer<CreateAbsenceCommandValidator> _localizer;

    public CreateAbsenceCommandValidator(IStringLocalizer<CreateAbsenceCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Start)
            .NotEmpty().WithMessage(_localizer["api.absence.create.startdate.required"])
            .LessThanOrEqualTo(v => v.End).WithMessage(_localizer["api.absence.create.startdate.lessThanEndDate"]);

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

public class CreateAbsenceCommandHandler : IRequestHandler<CreateAbsenceCommand, Result<Guid>>
{
    private readonly ICatalogDbContext _context;

    public CreateAbsenceCommandHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateAbsenceCommand request, CancellationToken cancellationToken)
    {
        var entity = new Absence
        {
            StartDate = request.Start,
            EndDate = request.End,
            Description = request.Description,
            Location = request.Location,
            Employee = request.Employee,
            CalendarId = request.CalendarId,
        };

        _context.Absences.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
