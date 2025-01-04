using System.Reflection;
using Ardalis.GuardClauses;
using Audit.EntityFramework;
using EvrenDev.Application.Common.Interfaces;
using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Interceptors;
using EvrenDev.Infrastructure.Catalog.Data;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using EvrenDev.Infrastructure.Tenant.Data;
using EvrenDev.Infrastructure.Tenant.Services;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using EvrenDev.Shared.Constants;
using EvrenDev.Infrastructure.Identity.Services;
using Audit.Core;
using EvrenDev.Infrastructure.Identity.Seed;
using EvrenDev.Infrastructure.Catalog.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        var catalogConnectionString = configuration.GetConnectionString("CatalogConnection");
        Guard.Against.Null(catalogConnectionString, message: "Catalog Connection string 'CatalogConnection' not found.");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
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
                    var user = services.BuildServiceProvider().GetService<ICurrentUserService>();
                    var ipAddress = services.BuildServiceProvider().GetService<IHttpContextAccessor>()?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

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
        services.AddDbContext<TenantDbContext>(options => options.UseSqlServer(tenantConnectionString));

        var identityConnectionString = configuration.GetConnectionString("IdentityConnection");
        Guard.Against.Null(identityConnectionString, message: "Identity Connection string 'IdentityConnection' not found.");
        services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(identityConnectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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
            options.AddPolicy(permission, policy => policy.RequireClaim(permission));
        }));

        services.AddDistributedMemoryCache();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton(TimeProvider.System);

        // Register database seeders
        services.AddScoped<IDatabaseSeeder, TenantDatabaseInitializer>();
        services.AddScoped<IDatabaseSeeder, IdentityDatabaseSeeder>();
        services.AddScoped<DevelopmentDatabaseSeeder>();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
