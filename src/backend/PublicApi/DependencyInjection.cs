
using Ardalis.GuardClauses;
using Audit.Core;
using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Data;
using EvrenDev.PublicApi.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        var auditConnectionString = configuration.GetConnectionString("AuditConnection");
        Guard.Against.Null(auditConnectionString, message: "Audit Connection string 'AuditConnection' not found.");

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
                    var user = services.BuildServiceProvider().GetService<IUser>();
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

        return services;
    }
}
