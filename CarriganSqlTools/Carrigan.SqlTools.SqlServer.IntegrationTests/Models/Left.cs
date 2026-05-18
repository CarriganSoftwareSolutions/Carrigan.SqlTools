namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class Left
{
    public int Id { get; set; }
    public string LeftWord { get; set; } = string.Empty;

    public static string CreateTableSql =>
        """
        CREATE TABLE [Left]
        (
            [Id] INT NOT NULL,
            [LeftWord] NVARCHAR(50) NOT NULL,
            CONSTRAINT [PK_Left] PRIMARY KEY ([Id])
        );
        """;
}
