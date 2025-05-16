using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Tenant.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Tenant.Seed;

public class TenantDatabaseSeeder : IDatabaseSeeder
{
    private readonly TenantDbContext _tenantDbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantDatabaseSeeder> _logger;

    public TenantDatabaseSeeder(
        TenantDbContext tenantDbContext,
        ILogger<TenantDatabaseSeeder> logger,
        IConfiguration configuration)
    {
        _tenantDbContext = tenantDbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting tenant database seeding...");

            if (_tenantDbContext.Tenants == null)
            {
                _logger.LogWarning("Tenants table not found in the database.");
                return;
            }

            var defaultTenant = _configuration.GetSection("DefaultTenant").Get<AppTenantInfo>();
            _logger.LogInformation("DefaultTenant configuration: {@DefaultTenant}", defaultTenant);

            if (defaultTenant == null)
            {
                _logger.LogWarning("DefaultTenant section not found in the configuration.");
                return;
            }

            var existingTenant = await _tenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == defaultTenant.Id);
            if (existingTenant != null)
            {
                _logger.LogInformation("Default tenant already exists with ID: {TenantId}", defaultTenant.Id);
                return;
            }

            await _tenantDbContext.Tenants.AddAsync(defaultTenant);
            await _tenantDbContext.SaveChangesAsync();

            _logger.LogInformation("Default tenant created successfully: {TenantName} (ID: {TenantId})",
                defaultTenant.Name, defaultTenant.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the tenant database: {Message}", ex.Message);
            throw;
        }
    }
}
