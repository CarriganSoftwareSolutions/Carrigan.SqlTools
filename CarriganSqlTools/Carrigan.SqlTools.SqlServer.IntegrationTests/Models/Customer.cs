using System;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

public sealed class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public char Gender { get; set; }

    public static string CreateTableSql => 
        """
        CREATE TABLE [Customer]
        (
            [Id] INT NOT NULL,
            [FirstName] NVARCHAR(50) NOT NULL,
            [LastName] NVARCHAR(50) NOT NULL,
            [Age] INT NOT NULL,
            [Gender] NCHAR(1) NOT NULL,
            CONSTRAINT [PK_Customer] PRIMARY KEY ([Id])
        );
        """;
}
