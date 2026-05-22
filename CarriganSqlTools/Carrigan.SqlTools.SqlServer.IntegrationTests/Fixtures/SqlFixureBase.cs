using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IntegrationTests.Fixtures;
using Carrigan.SqlTools.SqlGenerators;
using Microsoft.Data.SqlClient;
using Respawn;

// Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, dbo

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Fixtures;

public abstract class SqlFixtureBase : DbFixtureBase<SqlConnection>
{
    private const string DefaultSchemaName = "dbo";

    private static readonly SqlServerDialect Dialect = new();

    protected SqlFixtureBase(IEnumerable<string> tableDefinitions) :
        base(Configurations.MaintenanceDbConnectionString, tableDefinitions)
    {
    }

    protected SqlFixtureBase(IEnumerable<string> tableDefinitions, IEnumerable<SqlQuery> databaseSetups) :
        base(Configurations.MaintenanceDbConnectionString, tableDefinitions, databaseSetups)
    {
    }

    protected override IDbAdapter DbAdapter => Respawn.DbAdapter.SqlServer;

    protected override string SchemaName => DefaultSchemaName;

    protected override SqlConnection CreateConnection(string connectionString) => new(connectionString);

    protected override string BuildUnitTestConnectionString(string maintenanceConnectionString, string databaseName)
    {
        SqlConnectionStringBuilder builder = new(maintenanceConnectionString)
        {
            InitialCatalog = databaseName
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
        string dbNameLiteral = databaseName.Replace("'", "''");

        return $"""
            IF DB_ID(N'{dbNameLiteral}') IS NOT NULL
            BEGIN
                ALTER DATABASE {dbIdentifier} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE {dbIdentifier};
            END
            """;
    }

    protected override void ExecuteDatabaseSetup(SqlQuery query, SqlConnection connection) => 
        Commands.ExecuteNonQuery(query, null, connection);
}