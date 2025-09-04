using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Delete;

public class DeleteTenantCommand : IRequest<bool>
{
    public string TenantId { get; set; } = default!;

    public DeleteTenantCommand(string tenantId) => TenantId = tenantId;
}

public class DeleteTenantCommandValidator : CustomValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator() =>
        RuleFor(t => t.TenantId)
            .NotEmpty();
}

public class DeleteTenantCommandHandler(ITenantService tenantService) : IRequestHandler<DeleteTenantCommand, bool>
{
    public async Task<bool> Handle(DeleteTenantCommand command, CancellationToken cancellationToken) =>
        await tenantService.DeleteAsync(command.TenantId);
}
