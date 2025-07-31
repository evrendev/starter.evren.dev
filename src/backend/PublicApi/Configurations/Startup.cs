namespace EvrenDev.PublicApi.Configurations;

internal static class Startup
{
    internal static void AddConfigurations(this WebApplicationBuilder builder)
    {
        const string ConfigurationsDirectory = "Configurations";
        var env = builder.Environment;
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/logger.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/hangfire.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/hangfire.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/cache.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/cache.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/cors.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/cors.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/database.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/database.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/mail.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/mail.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/middleware.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/middleware.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/security.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/security.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/recaptcha.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/recaptcha.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/openapi.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/openapi.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/signalr.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/signalr.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/securityheaders.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{ConfigurationsDirectory}/securityheaders.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
    }
}
