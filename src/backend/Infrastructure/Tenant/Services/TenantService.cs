using EvrenDev.Application.Common.Services;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Tenant.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EvrenDev.Infrastructure.Tenant.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenantDbContext _context;
        private readonly IConfiguration _configuration;

        public TenantService(
            IHttpContextAccessor httpContextAccessor,
            TenantDbContext context,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
        }

        public string GetCurrentTenant()
        {
            var tenantId = _httpContextAccessor.HttpContext?.User?.FindFirst("tenant")?.Value;
            return tenantId ?? string.Empty;
        }

        public string GetCurrentTenantId()
        {
            return GetCurrentTenant();
        }

        public async Task<TenantEntity?> GetTenantAsync(string? tenantId)
        {
            if (Guid.TryParse(tenantId, out Guid guidTenantId))
            {
                return _context.Tenants != null
                    ? await _context.Tenants.FirstOrDefaultAsync(t => t.Id == guidTenantId)
                    : null;
            }

            return null;
        }

        public async Task<string?> GetConnectionStringAsync(string tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            if (tenant == null) return _configuration.GetConnectionString("DefaultConnection");
            return tenant.ConnectionString ?? _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> SetCurrentTenantAsync(string tenantId)
        {
            var tenant = await GetTenantAsync(tenantId);
            return tenant != null;
        }
    }
}
