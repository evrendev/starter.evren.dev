using EvrenDev.Domain.Multitenancy;

namespace EvrenDev.Infrastructure.Persistence.Initialization;

internal interface IDatabaseInitializer
{
    Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    Task InitializeApplicationDbForTenantAsync(TenantInfo tenant, CancellationToken cancellationToken);
}
