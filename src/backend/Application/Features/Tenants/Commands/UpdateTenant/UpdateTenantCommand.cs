namespace EvrenDev.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    private readonly IStringLocalizer<UpdateTenantCommandValidator> _localizer;

    public UpdateTenantCommandValidator(IStringLocalizer<UpdateTenantCommandValidator> localizer)
    {
        _localizer = localizer;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage(_localizer["api.tenants.update.id.required"]);

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage(_localizer["api.tenants.update.name.required"])
            .MaximumLength(200).WithMessage(_localizer["api.tenants.update.name.maxlength"]);
    }
}
public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, Result<bool>>
{
    private readonly ITenantDbContext _context;
    private readonly IStringLocalizer<UpdateTenantCommandHandler> _localizer;

    public UpdateTenantCommandHandler(
        ITenantDbContext context,
        IStringLocalizer<UpdateTenantCommandHandler> localizer)
    {
        _context = context;
        _localizer = localizer;
    }

    public async Task<Result<bool>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tenants.FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
            return Result<bool>.Failure(_localizer["api.tenants.not-found"]);

        entity.Name = request.Name;
        entity.ConnectionString = request.ConnectionString;
        entity.Host = request.Host;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
