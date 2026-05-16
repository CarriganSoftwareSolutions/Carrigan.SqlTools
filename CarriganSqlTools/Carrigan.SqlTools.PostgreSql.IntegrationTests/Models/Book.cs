namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

public sealed class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Pages { get; set; }
    public int YearPublished { get; set; }

    public static string CreateTableSql => 
        """
        CREATE TABLE "Book"
        (
            "Id" INTEGER NOT NULL,
            "Title" VARCHAR(200) NOT NULL,
            "Author" VARCHAR(150) NOT NULL,
            "Description" VARCHAR(1000) NOT NULL,
            "Price" NUMERIC(10,2) NOT NULL,
            "Pages" INTEGER NOT NULL,
            "YearPublished" INTEGER NOT NULL,
            CONSTRAINT "PK_Book" PRIMARY KEY ("Id")
        );
        """;
}
