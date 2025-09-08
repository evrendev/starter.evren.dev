using EvrenDev.Domain.Multitenancy;
using EvrenDev.Infrastructure.Multitenancy;
using EvrenDev.Infrastructure.Persistence.Context;
using EvrenDev.Shared.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EvrenDev.Migrators.Migrators.PostgreSQL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>, IDesignTimeDbContextFactory<ApplicationDbContext>
{
    TenantDbContext IDesignTimeDbContextFactory<TenantDbContext>.CreateDbContext(string[] args)
    {
        var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("backend/PublicApi/Configurations/database.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

        var builder = new DbContextOptionsBuilder<TenantDbContext>();
        builder.UseNpgsql(connectionString, e =>
            e.MigrationsAssembly("Migrators.PostgreSQL"));

        return new TenantDbContext(builder.Options);
    }

    ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
    {
        var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("backend/PublicApi/Configurations/database.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseNpgsql(connectionString, e =>
            e.MigrationsAssembly("Migrators.PostgreSQL"));

        // Varsayılan tenant oluşturuluyor
        var tenant = new TenantInfo(
            MultitenancyConstants.Root.Id,
            MultitenancyConstants.Root.Name,
            connectionString,
            MultitenancyConstants.Root.EmailAddress);

        return new ApplicationDbContext(
            tenant,
            builder.Options,
            null!, // ICurrentUser
            null!, // ISerializerService
            Microsoft.Extensions.Options.Options.Create(new EvrenDev.Infrastructure.Persistence.DatabaseSettings { DbProvider = "PostgreSQL" }), // DatabaseSettings
            null!); // IEventPublisher
    }
}
