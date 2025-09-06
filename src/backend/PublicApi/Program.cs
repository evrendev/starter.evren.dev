using EvrenDev.Application;
using EvrenDev.PublicApi.Configurations;
using EvrenDev.PublicApi.Controllers;
using EvrenDev.Infrastructure;
using EvrenDev.Infrastructure.Common;
using EvrenDev.Infrastructure.Common.Filters;
using EvrenDev.PublicApi;
using Serilog;

[assembly: ApiConventionType(typeof(ApiConventions))]

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations();
    builder.AddSerilog();

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<SortByBindingFilter>();
    });
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    var app = builder.Build();

    await app.Services.InitializeDatabasesAsync();

    app.UseInfrastructure(builder.Configuration);
    app.MapEndpoints();
    await app.RunAsync();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    await Log.CloseAndFlushAsync();
}
