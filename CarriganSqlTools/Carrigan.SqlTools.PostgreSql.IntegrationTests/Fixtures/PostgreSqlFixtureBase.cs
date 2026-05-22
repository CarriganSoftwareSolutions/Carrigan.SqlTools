using Carrigan.SqlTools.Clients.PostgreSql;
using Carrigan.SqlTools.Dialects.PostgreSql;
using Carrigan.SqlTools.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Npgsql;
using Respawn;

// Ignore Spelling: PostgreSql, Respawn, Respawner, Reseed, Carrigan, SqlTools, pgcrypto

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Fixtures;

public abstract class PostgreSqlFixtureBase : DbFixtureBase<NpgsqlConnection>
{
    private const string DefaultSchemaName = "public";

    private static readonly PostgreSqlDialect Dialect = new();

    protected PostgreSqlFixtureBase(IEnumerable<string> tableDefinitions) :
        base(Configurations.MaintenanceDbConnectionString, tableDefinitions)
    {
    }

    protected PostgreSqlFixtureBase(IEnumerable<string> tableDefinitions, IEnumerable<SqlQuery> databaseSetups) :
        base(Configurations.MaintenanceDbConnectionString, tableDefinitions, databaseSetups)
    {
    }

    protected override IDbAdapter DbAdapter => Respawn.DbAdapter.Postgres;

    protected override string SchemaName => DefaultSchemaName;

    protected override NpgsqlConnection CreateConnection(string connectionString) => new(connectionString);

    protected override string BuildUnitTestConnectionString(string maintenanceConnectionString, string databaseName)
    {
        NpgsqlConnectionStringBuilder builder = new(maintenanceConnectionString)
        {
            Database = databaseName
        };

        return builder.ConnectionString;
    }

    protected override string CreateDatabaseCommandText(string databaseName)
    {
        string dbIdentifier = Dialect.QuoteIdentifier(databaseName);

        return $"CREATE DATABASE {dbIdentifier};";
    }

    protected override string DropDatabaseCommandText(string databaseName)
    {
        string dbIdentifier = Dialect.QuoteIdentifier(databaseName);

        return $"""
            DROP DATABASE IF EXISTS {dbIdentifier} WITH (FORCE);
            """;
    }

    protected override async Task BeforeCreateTablesAsync(NpgsqlConnection connection) => 
        await ExecuteCommandAsync(connection, "CREATE EXTENSION IF NOT EXISTS pgcrypto;");

    protected override void ExecuteDatabaseSetup(SqlQuery query, NpgsqlConnection connection) => 
        Commands.ExecuteNonQuery(query, null, connection);
}