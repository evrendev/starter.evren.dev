using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;
using EvrenDev.PublicApi.Infrastructure;
using EvrenDev.PublicApi.Localization;
using EvrenDev.PublicApi.Middleware;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<CatalogDbContext>()
            .AddDbContextCheck<IdentityDbContext>()
            .AddDbContextCheck<AuditLogDbContext>()
            .AddDbContextCheck<TenantDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        // Add services to the container.
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        // Configure CORS
        services.AddCors(options =>
            options.AddPolicy("AllowSpecificOrigins",
                policy =>
                    policy.WithOrigins(
                        "https://donation.help-dunya.com:5001",
                        "https://donation.help-dunya.com:5002",
                        "http://localhost:5001",
                        "http://localhost:5002"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                )
        );

        services.AddLocalization();
        services.AddSingleton<LocalizationMiddleware>();
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "EvrenDev API",
                    Version = "v1",
                    TermsOfService = new Uri("https://donation.help-dunya.com:5002/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = new Uri("https://donation.help-dunya.com:5002/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://donation.help-dunya.com:5002/license")
                    }
                }
            )
        );

        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();

        return services;
    }
}
