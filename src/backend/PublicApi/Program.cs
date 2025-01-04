using EvrenDev.Infrastructure.Catalog.Services;
using EvrenDev.PublicApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
configuration.AddJsonFile("secret.json", optional: true, reloadOnChange: true);

builder.Services.AddApplicationServices(configuration);
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddWebServices(configuration);

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

app.UseHttpsRedirection();

app.UseHealthChecks("/health");

// Add CORS before authentication
app.UseCors("AllowSpecificOrigins");

// Add authentication before tenant middleware
app.UseAuthentication();

// Add tenant middleware before authorization
app.UseMiddleware<TenantMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
