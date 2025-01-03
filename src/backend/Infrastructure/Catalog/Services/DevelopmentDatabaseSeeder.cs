using EvrenDev.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace EvrenDev.Infrastructure.Catalog.Services;

public class DevelopmentDatabaseSeeder
{
    private readonly IEnumerable<IDatabaseSeeder> _seeders;
    private readonly ILogger<DevelopmentDatabaseSeeder> _logger;

    public DevelopmentDatabaseSeeder(
        IEnumerable<IDatabaseSeeder> seeders,
        ILogger<DevelopmentDatabaseSeeder> logger)
    {
        _seeders = seeders;
        _logger = logger;
    }

    public async Task SeedAllAsync()
    {
        foreach (var seeder in _seeders)
        {
            _logger.LogInformation("Running database seeder: {SeederName}", seeder.GetType().Name);
            await seeder.SeedAsync();
        }
    }
}
