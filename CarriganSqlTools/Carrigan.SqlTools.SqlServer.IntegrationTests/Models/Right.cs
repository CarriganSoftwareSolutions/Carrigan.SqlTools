namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class Right
{
    public int Id { get; set; }
    public string RightWord { get; set; } = string.Empty;

    public static string CreateTableSql =>
        """
        CREATE TABLE [Right]
        (
            [Id] INT NOT NULL,
            [RightWord] NVARCHAR(50) NOT NULL,
            CONSTRAINT [PK_Right] PRIMARY KEY ([Id])
        );
        """;
}