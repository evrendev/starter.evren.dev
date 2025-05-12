namespace EvrenDev.Application.Features.Absences.Commands.CreateAbsence;

public class CreateAbsenceCommand : IRequest<Result<Guid>>
{
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? Calendar { get; set; }
}

public class CreateAbsenceCommandValidator : AbstractValidator<CreateAbsenceCommand>
{
    private readonly IStringLocalizer<CreateAbsenceCommandValidator> _localizer;

    public CreateAbsenceCommandValidator(IStringLocalizer<CreateAbsenceCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage(_localizer["api.absence.create.title.required"])
            .MaximumLength(200).WithMessage(_localizer["api.absence.create.title.maxlength"]);

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage(_localizer["api.absence.create.startdate.required"])
            .LessThan(v => v.EndDate).WithMessage(_localizer["api.absence.create.startdate.lessThanEndDate"]);

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage(_localizer["api.absence.create.enddate.required"]);

        RuleFor(v => v.Employee)
            .NotEmpty().WithMessage(_localizer["api.absence.create.employee.required"])
            .MaximumLength(100).WithMessage(_localizer["api.absence.create.employee.maxlength"]);

        RuleFor(v => v.Calendar)
            .NotEmpty().WithMessage(_localizer["api.absence.create.calendar.required"])
            .MaximumLength(10).WithMessage(_localizer["api.absence.create.calendar.maxlength"]);

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
            Title = request.Title,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description,
            Location = request.Location,
            Employee = request.Employee,
            Calendar = request.Calendar,
        };

        _context.Absences.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id);
    }
}
