using EvrenDev.Application.Common.Exceptions;
using EvrenDev.Application.Common.Persistence;
using EvrenDev.Application.Multitenancy.Entities;
using EvrenDev.Application.Multitenancy.Interfaces;
using EvrenDev.Application.Multitenancy.Commands.Create;
using EvrenDev.Infrastructure.Persistence;
using EvrenDev.Infrastructure.Persistence.Initialization;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using EvrenDev.Application.Multitenancy.Commands.Update;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;
using EvrenDev.Application.Common.Models;
namespace EvrenDev.Infrastructure.Multitenancy;

internal class TenantService(
    IMultiTenantStore<TenantInfo> tenantStore,
    TenantDbContext tenantDbContext,
    IConnectionStringSecurer csSecurer,
    IDatabaseInitializer dbInitializer,
    IStringLocalizer<TenantService> localizer,
    IOptions<DatabaseSettings> dbSettings)
    : ITenantService
{
    private readonly DatabaseSettings _dbSettings = dbSettings.Value;
    private readonly TenantDbContext _dbContext = tenantDbContext;

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = (await tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();
        tenants.ForEach(t => t.ConnectionString = csSecurer.MakeSecure(t.ConnectionString));
        return tenants;
    }

    public async Task<bool> ExistsWithIdAsync(string id) =>
        await tenantStore.TryGetAsync(id) is not null;

    public async Task<bool> ExistsWithNameAsync(string name) =>
        (await tenantStore.GetAllAsync()).Any(t => t.Name == name);

    public async Task<TenantDto> GetByIdAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);
        var tenantDto = tenant.Adapt<TenantDto>();
        tenantDto.ConnectionString = csSecurer.MakeSecure(tenantDto.ConnectionString);
        tenantDto.ValidUpto = tenant.ValidUpto.ToString("yyyy-MM-dd");
        return tenantDto;
    }

    public async Task<string> CreateAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        if (command.ConnectionString?.Trim() == _dbSettings.ConnectionString?.Trim())
            command.ConnectionString = string.Empty;

        var tenant = new TenantInfo(command.Id, command.Name, command.ConnectionString, command.AdminEmail, command.Issuer);
        await tenantStore.TryAddAsync(tenant);

        try
        {
            await dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
        catch
        {
            await tenantStore.TryRemoveAsync(command.Id);
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> ActivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Activated.");
        }

        tenant.Activate();

        await tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id} is now Activated.";
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (!tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Deactivated.");
        }

        tenant.Deactivate();

        await tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id} is now Deactivated.";
    }

    public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate)
    {
        var tenant = await GetTenantInfoAsync(id);

        tenant.SetValidity(extendedExpiryDate);

        await tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id}'s Subscription Upgraded. Now Valid till {tenant.ValidUpto}.";
    }

    private async Task<TenantInfo> GetTenantInfoAsync(string id) =>
        await tenantStore.TryGetAsync(id)
            ?? throw new NotFoundException(string.Format(localizer["entity.notfound"], typeof(TenantInfo).Name, id));

    public async Task<string> UpdateAsync(UpdateTenantCommand command, CancellationToken cancellationToken)
    {
        if (command.ConnectionString?.Trim() == _dbSettings.ConnectionString?.Trim())
            command.ConnectionString = string.Empty;

        var tenant = new TenantInfo(id: command.Id, name: command.Name, connectionString: command.ConnectionString, adminEmail: command.AdminEmail, issuer: command.Issuer, isActive: command.IsActive, validUpto: command.ValidUpto);
        await tenantStore.TryUpdateAsync(tenant);

        try
        {
            await dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
        catch
        {
            await tenantStore.TryRemoveAsync(command.Id);
            throw;
        }

        return tenant.Id;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await tenantStore.TryRemoveAsync(id);
    }

    public async Task<PaginationResponse<TenantDto>> PaginatedListAsync(PaginationFilter filter, CancellationToken cancellationToken)
    {

        IQueryable<TenantInfo> query = _dbContext.TenantInfo.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
        {
            string searchLower = filter.Search.ToLower();
            query = query.Where(t =>
                t.Id.ToLower().Contains(searchLower) ||
                t.Name.ToLower().Contains(searchLower)
            );
        }

        if (filter.SortBy is { Length: > 0 })
        {
            bool isDescending = filter.SortDesc?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

            string sortField = filter.SortBy[0];

            switch (sortField.ToLower())
            {
                case "id":
                    query = isDescending ? query.OrderByDescending(t => t.Id) : query.OrderBy(t => t.Id);
                    break;
                case "name":
                    query = isDescending ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name);
                    break;
                default:
                    query = query.OrderBy(t => t.Id);
                    break;
            }
        }

        int totalItems = await query.CountAsync(cancellationToken);

        var pagedData = await query
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .ToListAsync(cancellationToken);

        var pagedDataDto = pagedData.Adapt<List<TenantDto>>();
        pagedDataDto.ForEach(t => t.ConnectionString = csSecurer.MakeSecure(t.ConnectionString));

        return new PaginationResponse<TenantDto>(pagedDataDto, totalItems, filter.Page, filter.ItemsPerPage);
    }
}
