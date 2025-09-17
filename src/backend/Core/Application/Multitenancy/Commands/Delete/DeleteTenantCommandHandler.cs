using EvrenDev.Application.Multitenancy.Interfaces;

namespace EvrenDev.Application.Multitenancy.Commands.Delete;

public class DeleteTenantCommand : IRequest<bool>
{
    public DeleteTenantCommand(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; } = default!;
}

public class DeleteTenantCommandValidator : CustomValidator<DeleteTenantCommand>
{
    public DeleteTenantCommandValidator()
    {
        RuleFor(t => t.TenantId)
            .NotEmpty();
    }
}

public class DeleteTenantCommandHandler(ITenantService tenantService) : IRequestHandler<DeleteTenantCommand, bool>
{
    public async Task<bool> Handle(DeleteTenantCommand command, CancellationToken cancellationToken)
    {
        return await tenantService.DeleteAsync(command.TenantId);
    }
}
