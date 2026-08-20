using EvrenDev.Infrastructure.Multitenancy;
using EvrenDev.Infrastructure.Persistence;
using EvrenDev.Infrastructure.Persistence.Context;
using EvrenDev.Shared.Multitenancy;
using Finbuckle.MultiTenant.Abstractions;
using TenantInfo = EvrenDev.Domain.Multitenancy.TenantInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EvrenDev.Migrators.Migrators.MSSQL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>,
    IDesignTimeDbContextFactory<ApplicationDbContext>
{
    ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
    {
        var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("backend/PublicApi/Configurations/database.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseSqlServer(connectionString, e =>
            e.MigrationsAssembly("Migrators.MSSQL"));

        // Varsayılan tenant oluşturuluyor
        var tenant = new TenantInfo(
            MultitenancyConstants.Root.Id,
            MultitenancyConstants.Root.Name,
            connectionString,
            MultitenancyConstants.Root.EmailAddress);

        var accessor = new AsyncLocalMultiTenantContextAccessor<TenantInfo>();
        ((IMultiTenantContextSetter)accessor).MultiTenantContext = new MultiTenantContext<TenantInfo>(tenant);

        return new ApplicationDbContext(
            accessor,
            builder.Options,
            null!, // ICurrentUser
            null!, // ISerializerService
            Options.Create(new DatabaseSettings { DbProvider = "SqlServer" }), // DatabaseSettings
            null!); // IEventPublisher
    }

    TenantDbContext IDesignTimeDbContextFactory<TenantDbContext>.CreateDbContext(string[] args)
    {
        var rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../"));
        var configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("backend/PublicApi/Configurations/database.json")
            .Build();

        var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;

        var builder = new DbContextOptionsBuilder<TenantDbContext>();
        builder.UseSqlServer(connectionString, e =>
            e.MigrationsAssembly("Migrators.MSSQL"));

        return new TenantDbContext(builder.Options);
    }
}
