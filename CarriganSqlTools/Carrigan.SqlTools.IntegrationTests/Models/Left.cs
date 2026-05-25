namespace Carrigan.SqlTools.IntegrationTests.Models;

public sealed class Left
{
    public int Id { get; set; }
    public string LeftWord { get; set; } = string.Empty;

    public static string CreateTableSqlServer =>
        """
        CREATE TABLE [Left]
        (
            [Id] INT NOT NULL,
            [LeftWord] NVARCHAR(50) NOT NULL,
            CONSTRAINT [PK_Left] PRIMARY KEY ([Id])
        );
        """;

    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "Left"
        (
            "Id" INTEGER NOT NULL,
            "LeftWord" VARCHAR(50) NOT NULL,
            CONSTRAINT "PK_Left" PRIMARY KEY ("Id")
        );
        """;
}
