//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, NVARCHAR, VARBINARY, DATETIME2, Xml

using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.IntegrationTests.Models;

[Identifier("Fields")]
public class FieldsModel
{
    // Primary key
    [PrimaryKey]
    public Guid Id { get; set; }

    // Identifiers
    public Guid GuidValue { get; set; }
    public Guid? GuidNullableValue { get; set; }

    // Text
    public string? StringValue { get; set; }
    public char CharValue { get; set; }
    public char? CharNullableValue { get; set; }

    // Integers
    public int IntValue { get; set; }
    public int? IntNullableValue { get; set; }
    public long LongValue { get; set; }
    public long? LongNullableValue { get; set; }
    public short ShortValue { get; set; }
    public short? ShortNullableValue { get; set; }
    public byte ByteValue { get; set; }
    public byte? ByteNullableValue { get; set; }

    // Boolean
    public bool BoolValue { get; set; }
    public bool? BoolNullableValue { get; set; }

    // Numeric (decimal / floating)
    public decimal DecimalValue { get; set; }
    public decimal? DecimalNullableValue { get; set; }
    public double DoubleValue { get; set; }
    public double? DoubleNullableValue { get; set; }
    public float FloatValue { get; set; }
    public float? FloatNullableValue { get; set; }

    // DateTime
    public DateOnly DateOnlyValue { get; set; }
    public DateOnly? DateOnlyNullableValue { get; set; }
    public TimeOnly TimeOnlyValue { get; set; }
    public TimeOnly? TimeOnlyNullableValue { get; set; }
    public DateTime DateTimeValue { get; set; }
    public DateTime? DateTimeNullableValue { get; set; }
    //TODO: Remove or replace TimeSpan code
    //public TimeSpan TimeSpanValue { get; set; }
    //public TimeSpan? TimeSpanNullableValue { get; set; }
    public DateTimeOffset DateTimeOffsetValue { get; set; }
    public DateTimeOffset? DateTimeOffsetNullableValue { get; set; }

    // Binary
    public byte[]? BytesValue { get; set; }

    // XML
    public System.Xml.Linq.XDocument? XDocumentValue { get; set; }
    public System.Xml.XmlDocument? XmlDocumentValue { get; set; }

    // SQL table DDL (used by the fixture)
    //TODO: Remove or replace TimeSpan code
    //TODO: revist decimal precision
    internal static string CreateTableSql =>
        """
        CREATE TABLE dbo.Fields
        (
            Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Fields PRIMARY KEY DEFAULT NEWID() ,

            GuidValue UNIQUEIDENTIFIER NOT NULL,
            GuidNullableValue UNIQUEIDENTIFIER NULL,

            StringValue NVARCHAR(200) NULL,

            CharValue NCHAR(1) NOT NULL,
            CharNullableValue NCHAR(1) NULL,

            IntValue INT NOT NULL,
            IntNullableValue INT NULL,

            LongValue BIGINT NOT NULL,
            LongNullableValue BIGINT NULL,

            ShortValue SMALLINT NOT NULL,
            ShortNullableValue SMALLINT NULL,

            ByteValue TINYINT NOT NULL,
            ByteNullableValue TINYINT NULL,

            BoolValue BIT NOT NULL,
            BoolNullableValue BIT NULL,

            DecimalValue DECIMAL(18,4) NOT NULL,
            DecimalNullableValue DECIMAL(18,4) NULL,

            -- C# double
            DoubleValue FLOAT(53) NOT NULL,        
            DoubleNullableValue FLOAT(53) NULL,

            -- C# float (single)
            FloatValue REAL NOT NULL,              
            FloatNullableValue REAL NULL,

            DateOnlyValue DATE NOT NULL,
            DateOnlyNullableValue DATE NULL,

            TimeOnlyValue TIME(7) NOT NULL,
            TimeOnlyNullableValue TIME(7) NULL,

            DateTimeValue DATETIME2(7) NOT NULL,
            DateTimeNullableValue DATETIME2(7) NULL,

            --TimeSpanValue BIGINT NOT NULL,
            --TimeSpanNullableValue BIGINT NULL,

            DateTimeOffsetValue DateTimeOffset(7) NOT NULL,
            DateTimeOffsetNullableValue DateTimeOffset(7) NULL,

            BytesValue VARBINARY(MAX) NULL,

            XDocumentValue XML NULL,
            XmlDocumentValue XML NULL
        );
        """;
}
