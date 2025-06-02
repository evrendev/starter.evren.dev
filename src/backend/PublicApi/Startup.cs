using Serilog;

namespace EvrenDev.PublicApi;

public static class Startup
{
    internal static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, config) =>
        {
            config.WriteTo.Console()
                .ReadFrom.Configuration(builder.Configuration);
        });
    }
}
