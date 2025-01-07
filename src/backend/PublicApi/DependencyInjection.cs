using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
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
                    policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                )
        );

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApi.Models.OpenApiInfo { Title = "EvrenDev API", Version = "v1" });
        });

        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();

        return services;
    }
}
