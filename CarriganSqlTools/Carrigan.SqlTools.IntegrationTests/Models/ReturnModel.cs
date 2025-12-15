using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations;
//IGNORE SPELLING: newid
namespace Carrigan.SqlTools.IntegrationTests.Models;

[Identifier("ReturnModel")]
internal class ReturnModel
{
    [PrimaryKey]
    //Should be auto id
    public int Id1 { get; set; }

    [PrimaryKey]
    //Should default to newid()
    public Guid Id2 { get; set; }

    public int NotKey1 { get; set; }
    public int NotKey2 { get; set; }
    [Alias("NotKey3Alias")]
    public int NotKey3 { get; set; }

    //Should Default to the sql system date/time
    public DateTime DateTime { get; set; }

    //Should default to "Pending"
    public string? Status { get; set; } = null;

    //Should default to true
    public bool DeletedFlag { get; set; }

    // SQL table DDL (used by the fixture)
    internal static string CreateTableSql =>
        """
        CREATE TABLE dbo.ReturnModel
        (
            Id1 INT IDENTITY(1,1) NOT NULL,
            Id2 UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ReturnModel_Id2 DEFAULT NEWID(),

            NotKey1 INT NOT NULL,
            NotKey2 INT NOT NULL,
            NotKey3 INT NOT NULL,

            [DateTime] DATETIME2(7) NOT NULL
                CONSTRAINT DF_ReturnModel_DateTime DEFAULT SYSUTCDATETIME(),

            Status NVARCHAR(50) NOT NULL
                CONSTRAINT DF_ReturnModel_Status DEFAULT N'Pending',

            DeletedFlag BIT NOT NULL
                CONSTRAINT DF_ReturnModel_DeletedFlag DEFAULT (0),

            CONSTRAINT PK_ReturnModel PRIMARY KEY (Id1, Id2)
        );
        """;
}
