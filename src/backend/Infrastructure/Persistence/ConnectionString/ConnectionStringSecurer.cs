using EvrenDev.Application.Common.Persistence;
using EvrenDev.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EvrenDev.Infrastructure.Persistence.ConnectionString;

public class ConnectionStringSecurer(IOptions<DatabaseSettings> dbSettings) : IConnectionStringSecurer
{
    private const string HiddenValueDefault = "*******";
    private readonly DatabaseSettings _dbSettings = dbSettings.Value;

    public string? MakeSecure(string? connectionString, string? dbProvider)
    {
        if (connectionString is null || string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        if (string.IsNullOrWhiteSpace(dbProvider))
        {
            dbProvider = _dbSettings.DbProvider;
        }

        return dbProvider?.ToLower() switch
        {
            DbProviderKeys.SqlServer => MakeSecureSqlServerConnectionString(connectionString),
            _ => connectionString
        };
    }

    private static string MakeSecureSqlServerConnectionString(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (!string.IsNullOrEmpty(builder.Password))
        {
            builder.Password = HiddenValueDefault;
        }

        if (!string.IsNullOrEmpty(builder.UserID))
        {
            builder.UserID = HiddenValueDefault;
        }

        return builder.ToString();
    }
}
