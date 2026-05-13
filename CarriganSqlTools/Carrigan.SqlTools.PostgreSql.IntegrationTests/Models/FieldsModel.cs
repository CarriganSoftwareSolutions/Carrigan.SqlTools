//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, NVARCHAR, VARBINARY, DATETIME2, Xml


//Ignore Spelling: Localdb, Respawn, Respawner, Reseed, Carrigan, SqlTools, NVARCHAR, VARBINARY, DATETIME2, Xml

using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.Models;

[Identifier("Fields")]
[Dialect(DialectEnum.PostgreSql)]
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

    public DateTimeOffset DateTimeOffsetValue { get; set; }
    public DateTimeOffset? DateTimeOffsetNullableValue { get; set; }

    // Binary
    public byte[]? BytesValue { get; set; }

    // XML
    public System.Xml.Linq.XDocument? XDocumentValue { get; set; }
    public System.Xml.XmlDocument? XmlDocumentValue { get; set; }
    public System.Xml.Linq.XDocument? XDocumentNullableValue { get; set; }
    public System.Xml.XmlDocument? XmlDocumentNullableValue { get; set; }

    // PostgreSQL table DDL (used by the fixture)
    internal static string CreateTableSql =>
        """
        CREATE EXTENSION IF NOT EXISTS pgcrypto;

        CREATE TABLE "Fields"
        (
            "Id" uuid NOT NULL DEFAULT gen_random_uuid(),

            "GuidValue" uuid NOT NULL,
            "GuidNullableValue" uuid NULL,

            "StringValue" varchar(200) NULL,

            "CharValue" char(1) NOT NULL,
            "CharNullableValue" char(1) NULL,

            "IntValue" integer NOT NULL,
            "IntNullableValue" integer NULL,

            "LongValue" bigint NOT NULL,
            "LongNullableValue" bigint NULL,

            "ShortValue" smallint NOT NULL,
            "ShortNullableValue" smallint NULL,

            "ByteValue" smallint NOT NULL,
            "ByteNullableValue" smallint NULL,

            "BoolValue" boolean NOT NULL,
            "BoolNullableValue" boolean NULL,

            "DecimalValue" numeric(18,4) NOT NULL,
            "DecimalNullableValue" numeric(18,4) NULL,

            -- C# double
            "DoubleValue" double precision NOT NULL,
            "DoubleNullableValue" double precision NULL,

            -- C# float / single
            "FloatValue" real NOT NULL,
            "FloatNullableValue" real NULL,

            "DateOnlyValue" date NOT NULL,
            "DateOnlyNullableValue" date NULL,

            "TimeOnlyValue" time(6) without time zone NOT NULL,
            "TimeOnlyNullableValue" time(6) without time zone NULL,

            "DateTimeValue" timestamp(6) without time zone NOT NULL,
            "DateTimeNullableValue" timestamp(6) without time zone NULL,

            "DateTimeOffsetValue" timestamp(6) with time zone NOT NULL,
            "DateTimeOffsetNullableValue" timestamp(6) with time zone NULL,

            "BytesValue" bytea NULL,

            "XDocumentValue" xml NULL,
            "XmlDocumentValue" xml NULL,

            "XDocumentNullableValue" xml NULL,
            "XmlDocumentNullableValue" xml NULL,

            CONSTRAINT "PK_Fields" PRIMARY KEY ("Id"),

            CONSTRAINT "CK_Fields_ByteValue"
                CHECK ("ByteValue" >= 0 AND "ByteValue" <= 255),

            CONSTRAINT "CK_Fields_ByteNullableValue"
                CHECK ("ByteNullableValue" IS NULL OR ("ByteNullableValue" >= 0 AND "ByteNullableValue" <= 255))
        );
        """;
}
