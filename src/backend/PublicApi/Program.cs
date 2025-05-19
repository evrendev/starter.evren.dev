using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Catalog.Services;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;
using EvrenDev.PublicApi.Extensions;
using EvrenDev.PublicApi.Hub;
using EvrenDev.PublicApi.Middleware;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

builder.Services.AddApplicationServices(configuration);
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddWebServices(configuration);

builder.Host.UseSerilog((context, conf) => conf.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Starting database migrations...");

        var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        logger.LogInformation("Migrating Catalog database...");
        await catalogDb.Database.MigrateAsync();
        logger.LogInformation("Catalog database migration completed.");

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        logger.LogInformation("Migrating Identity database...");
        await identityDb.Database.MigrateAsync();
        logger.LogInformation("Identity database migration completed.");

        var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        logger.LogInformation("Migrating Tenant database...");
        await tenantDb.Database.MigrateAsync();
        logger.LogInformation("Tenant database migration completed.");

        var auditDb = scope.ServiceProvider.GetRequiredService<AuditLogDbContext>();
        logger.LogInformation("Migrating Audit database...");
        await auditDb.Database.MigrateAsync();
        logger.LogInformation("Audit database migration completed.");

        logger.LogInformation("Starting database seeding...");
        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDatabaseSeeder>();
        await seeder.SeedAllAsync();
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while setting up the database.");
    }
}
else
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseMiddleware<LocalizationMiddleware>();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            currentTime,
            details = report.Entries.Select(e => new
            {
                key = e.Key,
                value = e.Value.Status.ToString(),
                description = e.Value.Description,
                data = e.Value.Data
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.UseCors("AllowSpecificOrigins");

app.UseMultiTenant();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandlerMiddleware();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationhub");

app.Run();
