namespace EvrenDev.Application.Catalog.Absences.Commands.UpdateAbsence;

public class UpdateAbsenceCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public string? Description { get; set; }
    public string Location { get; set; } = default!;
    public string Employee { get; set; } = default!;
    public string CalendarId { get; set; } = default!;
}

public class UpdateAbsenceCommandValidator : CustomValidator<UpdateAbsenceCommand>
{
    public UpdateAbsenceCommandValidator(IRepository<Brand> repository, IStringLocalizer<UpdateAbsenceCommandValidator> localizer)
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(localizer["api.absence.update.id.required"]);

        RuleFor(v => v.Start)
            .NotEmpty().WithMessage(localizer["api.absence.create.startdate.required"])
            .LessThan(v => v.End).WithMessage(localizer["api.absence.create.startdate.lessThanEndDate"]);

        RuleFor(v => v.End)
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

public class UpdateAbsenceCommandHandler(IRepositoryWithEvents<Brand> repository, IStringLocalizer<UpdateBrandRequestHandler> localizer) : IRequestHandler<UpdateAbsenceCommand, Guid>
{
    public async Task<Guid> Handle(UpdateAbsenceCommand command, CancellationToken cancellationToken)
    {
        var absence = await repository.GetByIdAsync(command.Id, cancellationToken);

        _ = absence ?? throw new NotFoundException(string.Format(localizer["api.absence.notfound"], request.Id));

        absence.Update(
            command.StartDate,
            command.EndDate,
            command.Description,
            command.Location,
            command.Employee,
            command.CalendarId
        );

        await repository.UpdateAsync(absence, cancellationToken);

        return command.Id;
    }
}
