//Ignore Spelling: pgcrypto

using Carrigan.SqlTools.Attributes;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.IntegrationTests.Models;

[Identifier("ArrayFields")]
public sealed class ArrayFieldsModel
{
    [PrimaryKey]
    public Guid Id { get; set; }

    public Guid[]? GuidArrayValue { get; set; }
    public Guid?[]? GuidNullableArrayValue { get; set; }

    public string?[]? StringArrayValue { get; set; }
    public char[]? CharArrayValue { get; set; }
    public char?[]? CharNullableArrayValue { get; set; }

    public byte[]?[]? BytesArrayValue { get; set; }

    public bool[]? BoolArrayValue { get; set; }
    public bool?[]? BoolNullableArrayValue { get; set; }

    public byte?[]? ByteNullableArrayValue { get; set; }
    public sbyte[]? SByteArrayValue { get; set; }
    public sbyte?[]? SByteNullableArrayValue { get; set; }
    public short[]? ShortArrayValue { get; set; }
    public short?[]? ShortNullableArrayValue { get; set; }
    public ushort[]? UShortArrayValue { get; set; }
    public ushort?[]? UShortNullableArrayValue { get; set; }
    public int[]? IntArrayValue { get; set; }
    public int?[]? IntNullableArrayValue { get; set; }
    public uint[]? UIntArrayValue { get; set; }
    public uint?[]? UIntNullableArrayValue { get; set; }
    public long[]? LongArrayValue { get; set; }
    public long?[]? LongNullableArrayValue { get; set; }
    public ulong[]? ULongArrayValue { get; set; }
    public ulong?[]? ULongNullableArrayValue { get; set; }

    public float[]? FloatArrayValue { get; set; }
    public float?[]? FloatNullableArrayValue { get; set; }
    public double[]? DoubleArrayValue { get; set; }
    public double?[]? DoubleNullableArrayValue { get; set; }
    public decimal[]? DecimalArrayValue { get; set; }
    public decimal?[]? DecimalNullableArrayValue { get; set; }

    public DateOnly[]? DateOnlyArrayValue { get; set; }
    public DateOnly?[]? DateOnlyNullableArrayValue { get; set; }
    public TimeOnly[]? TimeOnlyArrayValue { get; set; }
    public TimeOnly?[]? TimeOnlyNullableArrayValue { get; set; }
    public DateTime[]? DateTimeArrayValue { get; set; }
    public DateTime?[]? DateTimeNullableArrayValue { get; set; }
    public TimeSpan[]? TimeSpanArrayValue { get; set; }
    public TimeSpan?[]? TimeSpanNullableArrayValue { get; set; }
    public DateTimeOffset[]? DateTimeOffsetArrayValue { get; set; }
    public DateTimeOffset?[]? DateTimeOffsetNullableArrayValue { get; set; }

    public XDocument?[]? XDocumentArrayValue { get; set; }
    public XmlDocument?[]? XmlDocumentArrayValue { get; set; }
    public object?[]? ObjectArrayValue { get; set; }

    public static string CreateTablePostgreSql =>
        """
        CREATE EXTENSION IF NOT EXISTS pgcrypto;

        CREATE TABLE "ArrayFields"
        (
            "Id" uuid NOT NULL DEFAULT gen_random_uuid(),

            "GuidArrayValue" uuid[] NULL,
            "GuidNullableArrayValue" uuid[] NULL,

            "StringArrayValue" text[] NULL,
            "CharArrayValue" char(1)[] NULL,
            "CharNullableArrayValue" char(1)[] NULL,

            "BytesArrayValue" bytea[] NULL,

            "BoolArrayValue" boolean[] NULL,
            "BoolNullableArrayValue" boolean[] NULL,

            "ByteNullableArrayValue" smallint[] NULL,
            "SByteArrayValue" smallint[] NULL,
            "SByteNullableArrayValue" smallint[] NULL,
            "ShortArrayValue" smallint[] NULL,
            "ShortNullableArrayValue" smallint[] NULL,
            "UShortArrayValue" integer[] NULL,
            "UShortNullableArrayValue" integer[] NULL,
            "IntArrayValue" integer[] NULL,
            "IntNullableArrayValue" integer[] NULL,
            "UIntArrayValue" bigint[] NULL,
            "UIntNullableArrayValue" bigint[] NULL,
            "LongArrayValue" bigint[] NULL,
            "LongNullableArrayValue" bigint[] NULL,
            "ULongArrayValue" numeric(20, 0)[] NULL,
            "ULongNullableArrayValue" numeric(20, 0)[] NULL,

            "FloatArrayValue" real[] NULL,
            "FloatNullableArrayValue" real[] NULL,
            "DoubleArrayValue" double precision[] NULL,
            "DoubleNullableArrayValue" double precision[] NULL,
            "DecimalArrayValue" numeric[] NULL,
            "DecimalNullableArrayValue" numeric[] NULL,

            "DateOnlyArrayValue" date[] NULL,
            "DateOnlyNullableArrayValue" date[] NULL,
            "TimeOnlyArrayValue" time(6) without time zone[] NULL,
            "TimeOnlyNullableArrayValue" time(6) without time zone[] NULL,
            "DateTimeArrayValue" timestamp(6) without time zone[] NULL,
            "DateTimeNullableArrayValue" timestamp(6) without time zone[] NULL,
            "TimeSpanArrayValue" interval[] NULL,
            "TimeSpanNullableArrayValue" interval[] NULL,
            "DateTimeOffsetArrayValue" timestamp(6) with time zone[] NULL,
            "DateTimeOffsetNullableArrayValue" timestamp(6) with time zone[] NULL,

            "XDocumentArrayValue" xml[] NULL,
            "XmlDocumentArrayValue" xml[] NULL,
            "ObjectArrayValue" text[] NULL,

            CONSTRAINT "PK_PostgreSqlArrayFields" PRIMARY KEY ("Id")
        );
        """;
}
