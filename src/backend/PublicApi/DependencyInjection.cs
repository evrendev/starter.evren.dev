using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddDbContextCheck<IdentityDbContext>()
            .AddDbContextCheck<AuditLogDbContext>()
            .AddDbContextCheck<TenantDbContext>();

        // Add services to the container.
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        // Configure CORS
        services.AddCors(options =>
            options.AddPolicy("AllowSpecificOrigins",
                policy =>
                    policy.WithOrigins("https://*.evren.dev", "https://localhost:6000", "http://localhost:6001", "http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                )
        );

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "EvrenDev API", Version = "v1" });
        });

        return services;
    }
}
