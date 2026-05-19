using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class Left
{
    public int Id { get; set; }
    public string LeftWord { get; set; } = string.Empty;

    public static string CreateTableSql =>
        """
        CREATE TABLE "Left"
        (
            "Id" INTEGER NOT NULL,
            "LeftWord" VARCHAR(50) NOT NULL,
            CONSTRAINT "PK_Left" PRIMARY KEY ("Id")
        );
        """;
}
