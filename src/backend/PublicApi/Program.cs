using EvrenDev.Infrastructure.Catalog.Services;
using EvrenDev.PublicApi.Extensions;
using EvrenDev.PublicApi.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
configuration.AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

builder.Services.AddApplicationServices(configuration);
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddWebServices(configuration);

builder.Host.UseSerilog((context, conf) => conf.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed databases in development mode
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDatabaseSeeder>();
    await seeder.SeedAllAsync();
}
else
{
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseMiddleware<LocalizationMiddleware>();

app.UseHealthChecks("/health");

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
