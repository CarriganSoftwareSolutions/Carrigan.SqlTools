using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Dialect(DialectEnum.PostgreSql)]
public sealed class Right
{
    public int Id { get; set; }
    public string RightWord { get; set; } = string.Empty;

    public static string CreateTableSql =>
        """
        CREATE TABLE "Right"
        (
            "Id" INTEGER NOT NULL,
            "RightWord" VARCHAR(50) NOT NULL,
            CONSTRAINT "PK_Right" PRIMARY KEY ("Id")
        );
        """;
}