using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Domain.Catalog;

namespace EvrenDev.Application.Catalog.Absences.Commands.DeleteAbsence;

public class DeleteAbsenceCommand(Guid id) : IRequest<Guid>
{
    public Guid Id { get; set; } = id;
}

public class DeleteAbsenceCommandValidator : CustomValidator<DeleteAbsenceCommand>
{
    public DeleteAbsenceCommandValidator(IReadRepositoryBase<Brand> repository, IStringLocalizer<DeleteAbsenceCommandValidator> localizer)
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(localizer["api.absence.delete.id.required"]);
    }
}

public class DeleteAbsenceCommandHandler(IRepositoryWithEvents<Absence> repository,
    IStringLocalizer<DeleteAbsenceCommandHandler> localizer) : IRequestHandler<DeleteAbsenceCommand, Guid>
{
    public async Task<Guid> Handle(DeleteAbsenceCommand command, CancellationToken cancellationToken)
    {
        var absence = await repository.GetByIdAsync(command.Id, cancellationToken);

        _ = absence ?? throw new NotFoundException(localizer["api.absence.notfound"]);

        await repository.DeleteAsync(absence, cancellationToken);

        return command.Id;
    }
}
