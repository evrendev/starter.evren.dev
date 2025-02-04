using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Tenant.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Tenant.Services;

public class TenantDatabaseSeeder : IDatabaseSeeder
{
    private readonly TenantDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantDatabaseSeeder> _logger;

    public TenantDatabaseSeeder(
        TenantDbContext context,
        ILogger<TenantDatabaseSeeder> logger,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (_context.Tenants == null)
            {
                _logger.LogWarning("Tenants table not found in the database.");
                return;
            }
            ;

            var defaultTenant = _configuration.GetSection("DefaultTenant").Get<TenantEntity>();

            if (defaultTenant == null)
            {
                _logger.LogWarning("DefaultTenant section not found in the configuration.");
                return;
            }

            await _context.Tenants.AddAsync(defaultTenant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Default tenant created: {TenantName}", defaultTenant.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database.: {Message}", ex.Message);
        }
    }
}
