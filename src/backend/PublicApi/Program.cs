using EvrenDev.Infrastructure.Audit.Data;
using EvrenDev.Infrastructure.Catalog.Data;
using EvrenDev.Infrastructure.Catalog.Services;
using EvrenDev.Infrastructure.Identity.Data;
using EvrenDev.Infrastructure.Tenant.Data;
using EvrenDev.PublicApi.Extensions;
using EvrenDev.PublicApi.Middleware;
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
    var catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    catalogDb.Database.Migrate();

    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    identityDb.Database.Migrate();

    var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    tenantDb.Database.Migrate();

    var auditDb = scope.ServiceProvider.GetRequiredService<AuditLogDbContext>();
    auditDb.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDatabaseSeeder>();
    await seeder.SeedAllAsync();
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

// Add CORS before authentication
app.UseCors("AllowSpecificOrigins");

// Add authentication before tenant middleware
app.UseAuthentication();

app.UseAuthorization();

// Add tenant middleware before authorization
app.UseMiddleware<TenantMiddleware>();

app.UseExceptionHandlerMiddleware();

app.MapControllers();

app.Run();
