using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

[Identifier("Right")]
public sealed class RightWords
{
    public int Id { get; set; }
    public string RightWord { get; set; } = string.Empty;

    public static string CreateTableSqlServer =>
        """
        CREATE TABLE [Right]
        (
            [Id] INT NOT NULL,
            [RightWord] NVARCHAR(50) NOT NULL,
            CONSTRAINT [PK_Right] PRIMARY KEY ([Id])
        );
        """;

    public static string CreateTablePostgreSql =>
        """
        CREATE TABLE "Right"
        (
            "Id" INTEGER NOT NULL,
            "RightWord" VARCHAR(50) NOT NULL,
            CONSTRAINT "PK_Right" PRIMARY KEY ("Id")
        );
        """;
}