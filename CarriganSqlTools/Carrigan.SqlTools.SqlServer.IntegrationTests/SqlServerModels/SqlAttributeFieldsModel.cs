
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.SqlServerModels;

[Identifier("SqlAttributeFields")]
public sealed class SqlAttributeFieldsModel
{
    [PrimaryKey]
    public Guid Id { get; set; }

    [SqlChar(EncodingEnum.Ascii, StorageTypeEnum.Fixed, 4)]
    public string AsciiFixedCharValue { get; set; } = string.Empty;

    [SqlChar(EncodingEnum.Unicode, StorageTypeEnum.Var, 25)]
    public string? UnicodeVarCharValue { get; set; }

    [SqlVarCharMax(EncodingEnum.Ascii)]
    public string? VarCharMaxValue { get; set; }

    [SqlVarCharMax(EncodingEnum.Unicode)]
    public string? NVarCharMaxValue { get; set; }

    [SqlBinary(StorageTypeEnum.Fixed, 3)]
    public byte[]? BinaryValue { get; set; }

    [SqlBinary(StorageTypeEnum.Var, 5)]
    public byte[]? VarBinaryValue { get; set; }

    [SqlVarBinaryMax]
    public byte[]? VarBinaryMaxValue { get; set; }

    [SqlImage]
    public byte[]? ImageValue { get; set; }

    [SqlDate]
    public DateOnly DateValue { get; set; }

    [SqlDateTime(SizeableEnum.Regular)]
    public DateTime DateTimeValue { get; set; }

    [SqlDateTime2(3)]
    public DateTime DateTime2Value { get; set; }

    [SqlDateTimeOffset(3)]
    public DateTimeOffset DateTimeOffsetValue { get; set; }

    [SqlTime(3)]
    public TimeOnly TimeValue { get; set; }

    [SqlDecimal(10, 2)]
    public decimal DecimalValue { get; set; }

    [SqlFloat]
    public double FloatValue { get; set; }

    [SqlMoney(SizeableEnum.Regular)]
    public decimal MoneyValue { get; set; }

    [SqlMoney(SizeableEnum.Smaller)]
    public decimal SmallMoneyValue { get; set; }

    [SqlText(EncodingEnum.Ascii)]
    public string? TextValue { get; set; }

    [SqlText(EncodingEnum.Unicode)]
    public string? NTextValue { get; set; }

    public static string CreateTableSqlServer =>
        """
        CREATE TABLE dbo.SqlAttributeFields
        (
            Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_SqlAttributeFields PRIMARY KEY DEFAULT NEWID(),
            AsciiFixedCharValue CHAR(4) NOT NULL,
            UnicodeVarCharValue NVARCHAR(25) NULL,
            VarCharMaxValue VARCHAR(MAX) NULL,
            NVarCharMaxValue NVARCHAR(MAX) NULL,
            BinaryValue BINARY(3) NULL,
            VarBinaryValue VARBINARY(5) NULL,
            VarBinaryMaxValue VARBINARY(MAX) NULL,
            ImageValue IMAGE NULL,
            DateValue DATE NOT NULL,
            DateTimeValue DATETIME NOT NULL,
            DateTime2Value DATETIME2(3) NOT NULL,
            DateTimeOffsetValue DATETIMEOFFSET(3) NOT NULL,
            TimeValue TIME(3) NOT NULL,
            DecimalValue DECIMAL(10, 2) NOT NULL,
            FloatValue FLOAT(53) NOT NULL,
            MoneyValue MONEY NOT NULL,
            SmallMoneyValue SMALLMONEY NOT NULL,
            TextValue TEXT NULL,
            NTextValue NTEXT NULL
        );
        """;
}
