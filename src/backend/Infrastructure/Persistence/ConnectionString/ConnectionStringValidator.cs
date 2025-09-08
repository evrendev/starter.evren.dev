using EvrenDev.Application.Common.Persistence;
using EvrenDev.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EvrenDev.Infrastructure.Persistence.ConnectionString;

internal class ConnectionStringValidator(IOptions<DatabaseSettings> dbSettings, ILogger<ConnectionStringValidator> logger)
    : IConnectionStringValidator
{
    private readonly DatabaseSettings _dbSettings = dbSettings.Value;

    public bool TryValidate(string connectionString, string? dbProvider = null)
    {
        if (string.IsNullOrWhiteSpace(dbProvider))
        {
            dbProvider = _dbSettings.DbProvider;
        }

        try
        {
            switch (dbProvider?.ToUpperInvariant())
            {
                case DbProviderKeys.SqlServer:
                    var sqlservercs = new SqlConnectionStringBuilder(connectionString);
                    break;
                case DbProviderKeys.PostgreSQL:
                    var postgresqlcs = new NpgsqlConnectionStringBuilder(connectionString);
                    break;
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError($"Connection String Validation Exception : {ex.Message}");
            return false;
        }
    }
}
