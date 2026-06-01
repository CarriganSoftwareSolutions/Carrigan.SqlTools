namespace Carrigan.SqlTools.PostgreSql.IntegrationTests;

internal static class Configurations
{
    internal static readonly string MaintenanceDbConnectionString =
        Environment.GetEnvironmentVariable("CARRIGAN_SQLTOOLS_POSTGRES_CONNECTION")
            ?? throw new InvalidOperationException
                (
                    """
                    PostgreSQL integration test connection string was not found.
                    Set the CARRIGAN_SQLTOOLS_POSTGRES_CONNECTION environment variable.
                    """
                );
}