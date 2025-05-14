using System.Text;
using Application.Common.Interfaces;
using Ardalis.GuardClauses;
using Audit.Core;
using Audit.EntityFramework;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Domain.Entities.Identity;
using EvrenDev.Domain.Entities.Tenant;
using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Catalog.Interceptors;
using EvrenDev.Infrastructure.Catalog.Services;
using EvrenDev.Infrastructure.Donation.Data;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Identity.Interceptors;
using EvrenDev.Infrastructure.Identity.Seed;
using EvrenDev.Infrastructure.Identity.Services;
using EvrenDev.Infrastructure.Services;
using EvrenDev.Infrastructure.Tenant.Data;
using EvrenDev.Infrastructure.Tenant.Interceptors;
using EvrenDev.Infrastructure.Tenant.Seed;
using EvrenDev.Shared.Configurations;
using EvrenDev.Shared.Constants;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AuditableTenantInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, AuditableIdentityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var donationConnectionString = configuration.GetConnectionString("DonationConnection");
        Guard.Against.Null(donationConnectionString, message: "Donation Connection string 'DonationConnection' not found.");
        services.AddDbContext<DonationDbContext>((sp, options) => options.UseSqlServer(donationConnectionString));

        var catalogConnectionString = configuration.GetConnectionString("CatalogConnection");
        Guard.Against.Null(catalogConnectionString, message: "Catalog Connection string 'CatalogConnection' not found.");
        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.AddInterceptors(new AuditSaveChangesInterceptor());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(catalogConnectionString);
        });

        var auditConnectionString = configuration.GetConnectionString("AuditConnection");
        Guard.Against.Null(auditConnectionString, message: "Audit Connection string 'AuditConnection' not found.");
        services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(auditConnectionString));

        DbContextOptions<AuditLogDbContext> auditDbCtxOptions = new DbContextOptionsBuilder<AuditLogDbContext>()
            .UseSqlServer(auditConnectionString)
            .Options;

        Audit.Core.Configuration.Setup()
            .UseEntityFramework(_ => _
                .UseDbContext<AuditLogDbContext>(auditDbCtxOptions)
                .DisposeDbContext()
                .AuditTypeMapper(t => typeof(AuditLog))
                .AuditEntityAction<AuditLog>((ev, entry, entity) =>
                {
                    var user = services.BuildServiceProvider().GetService<ICurrentUser>();
                    var tenant = services.BuildServiceProvider().GetService<IMultiTenantContext<TenantEntity>>();
                    var ipAddress = services.BuildServiceProvider().GetService<IHttpContextAccessor>()?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

                    entity.Id = Guid.NewGuid();
                    entity.TenantId = tenant?.TenantInfo?.Id;
                    entity.Action = entry.Action;
                    entity.AuditData = entry.ToJson();
                    entity.EntityType = entry.EntityType.Name;
                    entity.AuditDateTimeUtc = DateTime.UtcNow;
                    entity.IpAddress = ipAddress;
                    entity.UserId = user?.Id;
                    entity.Email = user?.Email;
                    entity.FullName = user?.FullName;
                    entity.TablePk = entry.PrimaryKey.First().Value.ToString();

                    if (entry.Action == "Update")
                    {
                        bool? deleteNewValue =
                            (bool?)entry?.Changes?.FirstOrDefault(c => c.ColumnName == "Deleted")?.NewValue;
                        bool? deleteOriginalValue =
                            (bool?)entry?.Changes?.FirstOrDefault(c => c.ColumnName == "Deleted")?.OriginalValue;
                        bool originalIsDeleted = deleteOriginalValue != deleteNewValue;

                        entity.Action = originalIsDeleted && deleteNewValue == true
                            ? "Delete"
                            : originalIsDeleted && deleteNewValue == false
                                ? "Recovered"
                                : "Update";
                    }
                })
                .IgnoreMatchedProperties());

        var tenantConnectionString = configuration.GetConnectionString("TenantConnection");
        Guard.Against.Null(tenantConnectionString, message: "Tenant Connection string 'TenantConnection' not found.");
        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            options.AddInterceptors(new AuditSaveChangesInterceptor());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(tenantConnectionString);
        });

        var identityConnectionString = configuration.GetConnectionString("IdentityConnection");
        Guard.Against.Null(identityConnectionString, message: "Identity Connection string 'IdentityConnection' not found.");
        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.AddInterceptors(new AuditSaveChangesInterceptor());
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(identityConnectionString);
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Signin Options
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.SignIn.RequireConfirmedAccount = true;

            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.AllowedForNewUsers = false;

            // User settings.
            options.User.AllowedUserNameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+|~-={}[]:\";<>?,./";
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty))
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    // Skip the default logic
                    context.HandleResponse();

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        status = 401,
                        message = "You are not authorized"
                    });

                    await context.Response.WriteAsync(result);
                }
            };
        });

        var permissions = Policies.AllModulesWithPermissions.ToList();

        services.AddAuthorization(options => permissions.ForEach(permission =>
        {
            options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
        }));

        services.AddScoped<IDonationDbContext>(provider => provider.GetRequiredService<DonationDbContext>());
        services.AddScoped<ICatalogDbContext>(provider => provider.GetRequiredService<CatalogDbContext>());
        services.AddScoped<ITenantDbContext>(provider => provider.GetRequiredService<TenantDbContext>());
        services.AddScoped<IAuditLogDbContext>(provider => provider.GetRequiredService<AuditLogDbContext>());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITotpService, TotpService>();

        services.AddHttpClient();

        services.AddScoped<ISendmailService, SendmailService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton(TimeProvider.System);

        // Register database seeders
        services.AddScoped<IDatabaseSeeder, TenantDatabaseSeeder>();
        services.AddScoped<IDatabaseSeeder, IdentityDatabaseSeeder>();
        services.AddScoped<DevelopmentDatabaseSeeder>();

        // Add Cloudflare services
        services.Configure<CloudflareSettings>(configuration.GetSection("Cloudflare"));
        services.AddHttpClient("CloudflareImages");
        services.AddScoped<ICloudflareImageService, CloudflareImageService>();
        services.AddScoped<ICloudflareR2Service, CloudflareR2Service>();

        services.AddMultiTenant<TenantEntity>()
            .WithClaimStrategy("tenant_id")
            .WithEFCoreStore<TenantDbContext, TenantEntity>();

        return services;
    }
}
