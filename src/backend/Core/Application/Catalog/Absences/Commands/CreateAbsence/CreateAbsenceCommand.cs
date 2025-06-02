using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Commands.CreateAbsence;

public class CreateAbsenceCommand : IRequest<Guid>
{
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public string? Description { get; set; }
    public string Location { get; set; } = default!;
    public string Employee { get; set; } = default!;
    public string CalendarId { get; set; } = default!;
}

public class CreateAbsenceCommandValidator : CustomValidator<CreateAbsenceCommand>
{
    public CreateAbsenceCommandValidator(IReadRepositoryBase<Brand> repository, IStringLocalizer<CreateAbsenceCommandValidator> localizer)
    {
        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage(localizer["api.absence.create.startdate.required"])
            .LessThanOrEqualTo(v => v.EndDate).WithMessage(localizer["api.absence.create.startdate.lessThanEndDate"]);

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage(localizer["api.absence.create.enddate.required"]);

        RuleFor(v => v.Employee)
            .NotEmpty().WithMessage(localizer["api.absence.create.employee.required"])
            .MaximumLength(100).WithMessage(localizer["api.absence.create.employee.maxlength"]);

        RuleFor(v => v.CalendarId)
            .NotEmpty().WithMessage(localizer["api.absence.create.calendar.required"])
            .MaximumLength(100).WithMessage(localizer["api.absence.create.calendar.maxlength"]);

        RuleFor(v => v.Description)
            .MaximumLength(1000).WithMessage(localizer["api.absence.create.description.maxlength"]);

        RuleFor(v => v.Location)
            .MaximumLength(50).WithMessage(localizer["api.absence.create.location.maxlength"]);
    }
}

public class CreateAbsenceCommandHandler(IRepositoryWithEvents<Absence> repository) : IRequestHandler<CreateAbsenceCommand, Guid>
{
    public async Task<Guid> Handle(CreateAbsenceCommand command, CancellationToken cancellationToken)
    {
        var absence = new Absence(command.StartDate, command.EndDate, command.Location, command.Employee, command.CalendarId, command.Description);

        await repository.AddAsync(absence, cancellationToken);
        return absence.Id;
    }
}
