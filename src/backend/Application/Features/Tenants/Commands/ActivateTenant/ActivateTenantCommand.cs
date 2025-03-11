using Microsoft.EntityFrameworkCore;

namespace EvrenDev.Application.Features.Tenants.Commands.ActivateTenant;

public class ActivateTenantCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class ActivateTenantCommandValidator : AbstractValidator<ActivateTenantCommand>
{
    private readonly IStringLocalizer<ActivateTenantCommandValidator> _localizer;

    public ActivateTenantCommandValidator(IStringLocalizer<ActivateTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.activate.id.required"]);
    }
}

public class ActivateTenantCommandHandler : IRequestHandler<ActivateTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<ActivateTenantCommandHandler> _localizer;

    public ActivateTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<ActivateTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(ActivateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            return Result<bool>.Failure(_localizer["api.tenants.not-found"]);

        entity.Activate();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
