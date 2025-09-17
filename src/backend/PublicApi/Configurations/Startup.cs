namespace EvrenDev.PublicApi.Configurations;

internal static class Startup
{
    internal static void AddConfigurations(this WebApplicationBuilder builder)
    {
        const string ConfigurationsDirectory = "Configurations";
        var env = builder.Environment;
        builder.Configuration.AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/logger.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/logger.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/hangfire.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/hangfire.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cache.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cache.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cors.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/cors.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/database.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/database.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/mail.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/mail.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/middleware.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/middleware.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/security.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/security.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/recaptcha.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/recaptcha.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/openapi.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/openapi.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/signalr.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/signalr.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($"{ConfigurationsDirectory}/securityheaders.json", false, true)
            .AddJsonFile($"{ConfigurationsDirectory}/securityheaders.{env.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
    }
}
