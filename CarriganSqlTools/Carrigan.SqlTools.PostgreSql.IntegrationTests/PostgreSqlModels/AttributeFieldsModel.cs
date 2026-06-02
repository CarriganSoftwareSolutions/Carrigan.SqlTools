//Ignore Spelling: PostgreSql, varchar

using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.PostgreSql.IntegrationTests.PostgreSqlModels;

[Identifier("AttributeFields")]
public sealed class AttributeFieldsModel
{
    [PrimaryKey]
    public Guid Id { get; set; }

    [PostgreSqlChar(StorageTypeEnum.Fixed, 4)]
    public string FixedCharValue { get; set; } = string.Empty;

    [PostgreSqlChar(StorageTypeEnum.Var, 25)]
    public string? VarCharValue { get; set; }

    [PostgreSqlFloat(24)]
    public float FloatValue { get; set; }

    [PostgreSqlMoney]
    public decimal MoneyValue { get; set; }

    [PostgreSqlNumeric(10, 2)]
    public decimal NumericValue { get; set; }

    [PostgreSqlDate]
    public DateOnly DateValue { get; set; }

    [PostgreSqlTime(3)]
    public TimeOnly TimeValue { get; set; }

    [PostgreSqlTimestamp(3)]
    public DateTime TimestampValue { get; set; }

    [PostgreSqlTimestampUtc(3)]
    public DateTimeOffset TimestampUtcValue { get; set; }

    [PostgreSqlNumeric(10, 2)]
    public decimal?[]? NumericArrayValue { get; set; }

    [PostgreSqlChar(StorageTypeEnum.Var, 25)]
    public string?[]? VarCharArrayValue { get; set; }

    public static string CreateTablePostgreSql =>
        """
        CREATE EXTENSION IF NOT EXISTS pgcrypto;

        CREATE TABLE "AttributeFields"
        (
            "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
            "FixedCharValue" char(4) NOT NULL,
            "VarCharValue" varchar(25) NULL,
            "FloatValue" float(24) NOT NULL,
            "MoneyValue" money NOT NULL,
            "NumericValue" numeric(10, 2) NOT NULL,
            "DateValue" date NOT NULL,
            "TimeValue" time(3) without time zone NOT NULL,
            "TimestampValue" timestamp(3) without time zone NOT NULL,
            "TimestampUtcValue" timestamp(3) with time zone NOT NULL,
            "NumericArrayValue" numeric(10, 2)[] NULL,
            "VarCharArrayValue" varchar(25)[] NULL,

            CONSTRAINT "PK_PostgreSqlAttributeFields" PRIMARY KEY ("Id")
        );
        """;
}
