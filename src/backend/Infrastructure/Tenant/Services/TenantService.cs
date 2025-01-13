using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Tenant.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Tenant.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            IHttpContextAccessor httpContextAccessor,
            TenantDbContext context,
            IConfiguration configuration,
            ILogger<TenantService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public Guid? GetCurrentTenantId()
        {
            var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant")?.Value;
            _logger.LogDebug("Current tenant ID from claims: {TenantId}", tenantId ?? "none");
            return tenantId != null && Guid.TryParse(tenantId, out Guid result) ? result : null;
        }

        public async Task<TenantEntity?> GetTenantAsync(Guid? tenantId)
        {
            return await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<string?> GetConnectionStringAsync(Guid? tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return _configuration.GetConnectionString("DefaultConnection");
            return tenant.ConnectionString ?? _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> SetCurrentTenantAsync(Guid? tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            return tenant != null;
        }

        public Guid? GetDefaultTenantId()
        {
            var tenantId = _configuration["DefaultTenant:Id"];
            return tenantId != null && Guid.TryParse(tenantId, out Guid result) ? result : null;
        }
    }
}
