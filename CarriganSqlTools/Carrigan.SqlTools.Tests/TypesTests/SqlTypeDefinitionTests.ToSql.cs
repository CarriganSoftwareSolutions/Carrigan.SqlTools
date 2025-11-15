using System.Data;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TypesTests;

public sealed partial class SqlTypeDefinitionTests
{
    [Theory]
    [InlineData(SqlDbType.UniqueIdentifier, "UNIQUEIDENTIFIER")]
    [InlineData(SqlDbType.Char, "CHAR")]
    [InlineData(SqlDbType.NChar, "NCHAR")]
    [InlineData(SqlDbType.VarChar, "VARCHAR")]
    [InlineData(SqlDbType.NVarChar, "NVARCHAR")]
    [InlineData(SqlDbType.VarBinary, "VARBINARY")]
    [InlineData(SqlDbType.Bit, "BIT")]
    [InlineData(SqlDbType.Int, "INT")]
    [InlineData(SqlDbType.BigInt, "BIGINT")]
    [InlineData(SqlDbType.Real, "REAL")]
    [InlineData(SqlDbType.Float, "FLOAT")]
    [InlineData(SqlDbType.Decimal, "DECIMAL")]
    [InlineData(SqlDbType.Money, "MONEY")]
    [InlineData(SqlDbType.SmallMoney, "SMALLMONEY")]
    [InlineData(SqlDbType.Date, "DATE")]
    [InlineData(SqlDbType.DateTime, "DATETIME")]
    [InlineData(SqlDbType.SmallDateTime, "SMALLDATETIME")]
    [InlineData(SqlDbType.DateTime2, "DATETIME2")]
    [InlineData(SqlDbType.Time, "TIME")]
    [InlineData(SqlDbType.DateTimeOffset, "DATETIMEOFFSET")]
    [InlineData(SqlDbType.Timestamp, "ROWVERSION")]
    [InlineData(SqlDbType.Variant, "SQL_VARIANT")]
    public void ToSql_MapsSqlDbType_ToExpectedToken(SqlDbType input, string expected) =>
        Assert.Equal(expected, SqlTypeDefinition.ToSql(input));

    [Theory]
    [InlineData(SqlDbType.Udt)]
    [InlineData(SqlDbType.Structured)]
    public void ToSql_ThrowsForUnsupportedTokens(SqlDbType unsupported) =>
        Assert.Throws<SqlTypeNotSupportedException>(() => SqlTypeDefinition.ToSql(unsupported));
}
