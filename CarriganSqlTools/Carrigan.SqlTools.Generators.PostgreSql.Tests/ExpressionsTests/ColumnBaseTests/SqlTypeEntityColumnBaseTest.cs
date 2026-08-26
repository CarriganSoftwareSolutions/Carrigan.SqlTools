using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests.ColumnBaseTests;

public class SqlTypeEntityColumnBaseTest : PostgreSqlColumnBaseTest<SqlTypeEntity>
{
    protected override string? SchemaName =>
        null;

    protected override string TableName =>
        "TestSqlTypes";

    protected override IEnumerable<string> NumericProperties =>
        [
            nameof(SqlTypeEntity.IntValue),
            nameof(SqlTypeEntity.LongValue),
            nameof(SqlTypeEntity.ShortValue),
            nameof(SqlTypeEntity.ByteValue),
            nameof(SqlTypeEntity.DecimalValue),
            nameof(SqlTypeEntity.FloatValue),
            nameof(SqlTypeEntity.DoubleValue)
        ];

    protected override IEnumerable<string> BooleanProperties =>
        [nameof(NullableTestEntity.BoolValue)];

    internal override Dictionary<string, ColumnName> ExpectedPropertyColumnName =>
    new
    (
        [
            NewKvp(nameof(SqlTypeEntity.IntValue)),
            NewKvp(nameof(SqlTypeEntity.LongValue)),
            NewKvp(nameof(SqlTypeEntity.ShortValue)),
            NewKvp(nameof(SqlTypeEntity.ByteValue)),
            NewKvp(nameof(SqlTypeEntity.BoolValue)),
            NewKvp(nameof(SqlTypeEntity.DecimalValue)),
            NewKvp(nameof(SqlTypeEntity.FloatValue)),
            NewKvp(nameof(SqlTypeEntity.DoubleValue)),
            NewKvp(nameof(SqlTypeEntity.StringValue)),
            NewKvp(nameof(SqlTypeEntity.DateTimeValue)),
            NewKvp(nameof(SqlTypeEntity.GuidValue)),
            NewKvp(nameof(SqlTypeEntity.ByteArrayValue)),
            NewKvp(nameof(SqlTypeEntity.CharValue)),
            NewKvp(nameof(SqlTypeEntity.TimeOnlyValue)),
            NewKvp(nameof(SqlTypeEntity.DateOnlyValue)),
            NewKvp(nameof(SqlTypeEntity.DateTimeOffsetValue)),
        ]
    );

    protected override ColumnBase NewColumn(string propertyName) =>
        new Column<SqlTypeEntity>(propertyName);

    protected override ColumnBase NewColumn(PropertyName propertyName) =>
        new Column<SqlTypeEntity>(propertyName);
}