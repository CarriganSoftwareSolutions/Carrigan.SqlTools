namespace Carrigan.SqlTools.SqlServer.IntegrationTests;
internal static class Configurations
{
    internal static readonly string MaintenanceDbConnectionString =
        Environment.GetEnvironmentVariable("CARRIGAN_SQLTOOLS_SQLSERVER_CONNECTION")
            ?? throw new InvalidOperationException
                (
                    """
                    SQL Server integration test connection string was not found.
                    Set the CARRIGAN_SQLTOOLS_SQLSERVER_CONNECTION environment variable.
                    """
                );
}
