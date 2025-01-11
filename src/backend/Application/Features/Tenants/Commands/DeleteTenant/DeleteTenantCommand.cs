namespace EvrenDev.Application.Features.Tenants.Commands.DeleteTenant;

public class DeleteTenantCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteTenantCommandValidator : AbstractValidator<DeleteTenantCommand>
{
    private readonly IStringLocalizer<DeleteTenantCommandValidator> _localizer;

    public DeleteTenantCommandValidator(IStringLocalizer<DeleteTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.delete.id.required"]);
    }
}

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<DeleteTenantCommandHandler> _localizer;

    public DeleteTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<DeleteTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            return Result<bool>.Failure(_localizer["api.tenants.not-found"]);

        _context.Tenants.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
